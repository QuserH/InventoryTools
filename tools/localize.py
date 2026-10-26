# -*- coding: utf-8 -*-
"""
InventoryTools 汉化工具（AutoRetainer-zh 同款流程）

在上游合并之后运行，把源码中「UI 展示上下文」里的英文串按 tools/zh-CN.json
词典替换成中文。已经含中文的串会跳过，因此可重复运行（幂等）。

用法：
    python tools/localize.py            # 应用
    python tools/localize.py --report   # 只统计，不写文件

设计要点：
- 只翻译 UI 展示上下文（ImGui / 自定义列与设置 DSL / Name/HelpText 等文本属性 /
  [HelpMessage] 特性）。逻辑比较、命令、IPC、日志、导入导出协议不在范围内。
- 保留 ImGui 的 ## / ### 隐藏 ID，只翻译可见部分。
- Lists/ListImportExportService.cs 与 Localization 字典文件整体跳过。
"""
import io
import json
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC = os.path.join(ROOT, "InventoryTools")
DICT_PATH = os.path.join(ROOT, "tools", "zh-CN.json")

SKIP_FILES = {
    os.path.normpath("InventoryTools/Localization/LocalizationService.cs"),
    os.path.normpath("InventoryTools/Localization/UiTranslations.cs"),
    os.path.normpath("InventoryTools/Lists/ListImportExportService.cs"),
}

# 这些串即使出现在 UI 调用里也不能翻（协议 token / 资源键 / 逻辑占位符）
SKIP_STRINGS = {
    "Items :", "(HQ)", " (HQ)", "(NQ)", " (NQ)", "item/", "1x",
    "***ID***", "***Name***", "ct_", "se.", "dalamud.log",
    "wrench-icon", "icon-hor", "DraggerBtn", "Ko-Fi",
    "NPC", "NPCs", "Npcs", "PVP", "ID", "=", "[", "-", "→",
    "5Label", "6Label", "SpinLabel", "Normal Quality", "High Quality",
}

UI_METHODS = {
    # Dalamud ImGui（最后一个方法名，匹配时统一小写）
    "text", "textunformatted", "textwrapped", "textdisabled", "textcolored",
    "button", "smallbutton", "arrowbutton", "imagebutton", "invisiblebutton",
    "menuitem", "menubutton", "labeltext", "collapsingheader", "selectable",
    "treenode", "tabitem", "checkbox", "radiobutton", "inputtext",
    "inputtextwithhint", "inputint", "inputfloat", "inputdouble",
    "sliderint", "sliderfloat", "combo", "begincombo", "listbox", "beginlistbox",
    "settooltip", "setnextitemtooltip", "tableheader", "tablesetupcolumn",
    "bullettext", "header", "subheader", "separatortext", "textcopyable",
    # OtterGui / ImRaii / 本项目自定义
    "hovertooltip", "helppopup", "helpmarker", "verticalaligntext",
    "addstringcolumn", "addintegercolumn", "addbooleancolumn", "addfloatcolumn",
    "addcompendiumopenviewcolumn", "additemsourcescolumn", "addsection", "addfilter",
    "addtext", "addhelp", "addtooltip", "addtab", "addlabel",
    "drawtext", "drawbutton", "drawsection", "drawhelp", "drawlabel",
    "infodialog", "confirmdialog", "notify", "notifyerror", "notifysuccess",
    "notifywarning", "success", "warning", "error", "info",
    "textcentered", "linecentered", "texttitled", "label", "title",
    "format", "string.format",
}

# 文本属性：表达式体（Name => "..."）或对象初始化/赋值（Name = "..."）
TEXT_PROPS = (
    "Name", "Plural", "Singular", "Description", "HelpText", "SectionName",
    "RenderName", "Label", "Title", "Tooltip", "HeaderName", "DisplayName",
    "ColumnName", "FilterName", "StatusText", "Message", "Text",
)

PROP_CONTEXT = re.compile(
    r'\b(' + "|".join(TEXT_PROPS) + r')\s*(?:=>|=)\s*$')
ASSIGN_CTX = re.compile(r'([A-Za-z_]\w*)\s*=\s*$')
ARROW_CTX = re.compile(r'([A-Za-z_]\w*)\s*=>\s*$')

BLACK_METHODS = {
    "equals", "equalsany", "equalsignorecase", "equalsignorecaseany", "contains",
    "containsany", "startswith", "startswithany", "endswith", "endswithany",
    "indexof", "compareto", "throttle", "pushid", "popid", "beginpopup",
    "openpopup", "closepopup", "begin", "beginchild", "setclipboardtext",
    "getconfig", "setconfig", "tostring", "parse", "getname", "getnames",
    "getfield", "getproperty", "addtag", "gettag", "hastag", "find", "where",
    "select", "first", "firstordefault", "any", "all", "count", "getrow",
    "gettext", "cleanup", "todalamudstring", "split", "join", "replace",
    "trim", "trimstart", "trimend", "toupper", "tolower", "substring",
    "concat", "trygetvalue", "getoradd", "containskey", "containsvalue",
    "getsheet", "getexcel", "executecommand", "processcommand", "getaddon",
    "trygetaddon", "validate", "log", "debug", "print", "write", "warn",
    "trace", "ismatch", "matches", "gettype", "create", "instantiate",
    "getvalue", "getint", "getbool", "getstring", "getuint", "getfloat",
    "getlong", "getulong", "getbyte", "getshort", "getushort", "tryget",
    "getindex", "lastindexof", "getfilename", "getdirectoryname", "combine",
    "getfullpath", "getextension", "getcurrentdirectory", "getkey",
    "patch", "hook", "sendmessage", "invoke",
}

with io.open(DICT_PATH, encoding="utf-8-sig") as f:
    DICT = json.load(f)


def has_cjk(s):
    return any("\u4e00" <= c <= "\u9fff" for c in s)


def split_id(s):
    i = s.find("###")
    if i < 0:
        i = s.find("##")
    if i < 0:
        return s, ""
    return s[:i], s[i:]


def unescape(s):
    out = []
    i = 0
    while i < len(s):
        c = s[i]
        if c == "\\" and i + 1 < len(s):
            n = s[i + 1]
            m = {"n": "\n", "r": "\r", "t": "\t", '"': '"', "'": "'", "\\": "\\", "0": "\0"}.get(n)
            if m is not None:
                out.append(m); i += 2; continue
            if n == "u" and i + 5 < len(s):
                try:
                    out.append(chr(int(s[i + 2:i + 6], 16))); i += 6; continue
                except ValueError:
                    pass
        out.append(c); i += 1
    return "".join(out)


def escape(s):
    return (s.replace("\\", "\\\\").replace('"', '\\"')
             .replace("\n", "\\n").replace("\r", "\\r").replace("\t", "\\t"))


def tokenize(text):
    tokens = []
    i = 0
    n = len(text)
    buf = []

    def flush():
        if buf:
            tokens.append(("code", "".join(buf)))
            buf.clear()

    while i < n:
        c = text[i]
        if c == "/" and i + 1 < n and text[i + 1] == "/":
            flush()
            j = text.find("\n", i)
            j = n if j < 0 else j
            tokens.append(("comment", text[i:j])); i = j; continue
        if c == "/" and i + 1 < n and text[i + 1] == "*":
            flush()
            j = text.find("*/", i + 2)
            j = n if j < 0 else j + 2
            tokens.append(("comment", text[i:j])); i = j; continue
        if text.startswith('"""', i):
            flush()
            j = text.find('"""', i + 3)
            j = n if j < 0 else j + 3
            tokens.append(("raw_string", text[i:j])); i = j; continue
        if c == '"':
            flush()
            j = i + 1
            while j < n:
                if text[j] == "\\":
                    j += 2; continue
                if text[j] == '"':
                    j += 1; break
                j += 1
            tokens.append(("string", text[i:j])); i = j; continue
        if c == "'":
            flush()
            j = i + 1
            while j < n:
                if text[j] == "\\":
                    j += 2; continue
                if text[j] == "'":
                    j += 1; break
                j += 1
            tokens.append(("char", text[i:j])); i = j; continue
        buf.append(c); i += 1
    flush()
    return tokens


def get_call_context(text, start):
    """Return (enclosing_method_name, unclosed_brace_depth_at_that_call).

    unclosed_brace_depth > 0 means the literal sits inside an object/collection
    initializer or lambda, so the outer method name alone is not enough evidence
    that the literal is UI text (e.g. new(){Key = "icon"})."""
    prefix = text[:start]
    depth = 0
    curly = 0
    i = len(prefix) - 1
    while i >= 0:
        c = prefix[i]
        if c == ")":
            depth += 1
        elif c == "}":
            curly += 1
        elif c == "{":
            if curly > 0:
                curly -= 1
        elif c == "(":
            if depth > 0:
                depth -= 1
            else:
                m = re.search(r"([A-Za-z_][\w]*(?:\.[A-Za-z_][\w]*)*)\s*$", prefix[:i])
                name = m.group(1).split(".")[-1].lower() if m else None
                return name, curly
        i -= 1
    return None, curly


def get_prop_context(text, start):
    line_start = text.rfind("\n", 0, start) + 1
    prefix = text[line_start:start]
    return bool(PROP_CONTEXT.search(prefix))


def get_help_message_context(text, start):
    line_start = text.rfind("\n", 0, start) + 1
    prefix = text[line_start:start]
    return prefix.rstrip().endswith("[HelpMessage(")


def translate_content(content):
    if not content or has_cjk(content):
        return None
    if content in SKIP_STRINGS:
        return None
    visible, ident = split_id(content)
    if not visible or visible in SKIP_STRINGS:
        return None
    hit = DICT.get(visible)
    if hit is None:
        return None
    return hit + ident


def process_file(path):
    raw = open(path, "rb").read()
    bom = raw.startswith(b"\xef\xbb\xbf")
    text = raw.decode("utf-8-sig")
    tokens = tokenize(text)
    pos = 0
    out = []
    changed = 0
    misses = []
    applied = []

    for kind, tok in tokens:
        if kind in ("comment", "code", "char"):
            out.append(tok); pos += len(tok); continue

        is_raw = kind == "raw_string"
        if is_raw:
            content = tok[3:-3]
            quote = lambda s: '"""' + s + '"""'
        else:
            raw_content = tok[1:-1]
            content = unescape(raw_content)
            quote = lambda s: '"' + escape(s) + '"'

        method, _curly = get_call_context(text, pos)
        prop = get_prop_context(text, pos)
        help_attr = get_help_message_context(text, pos)

        # "Key = "icon"" / "row => "x"" are NOT UI text unless the property
        # name is one of the known text properties.
        line_start = text.rfind("\n", 0, pos) + 1
        prefix = text[line_start:pos]
        m_assign = ASSIGN_CTX.search(prefix)
        m_arrow = ARROW_CTX.search(prefix)
        assign_name = m_assign.group(1) if m_assign else None
        arrow_name = m_arrow.group(1) if m_arrow else None
        is_text_assign = assign_name in TEXT_PROPS if assign_name else False
        is_text_arrow = arrow_name in TEXT_PROPS if arrow_name else False
        blocked_by_assign = (assign_name is not None or arrow_name is not None) \
            and not (is_text_assign or is_text_arrow)

        translated = None
        if help_attr:
            translated = translate_content(content)
        elif is_text_assign or is_text_arrow:
            translated = translate_content(content)
        elif blocked_by_assign:
            translated = None
        elif method in UI_METHODS:
            translated = translate_content(content)

        if translated is not None:
            out.append(quote(translated)); changed += 1
            applied.append((text.count("\n", 0, pos) + 1, content, translated))
        else:
            out.append(tok)
            in_ui = help_attr or is_text_assign or is_text_arrow or \
                (method in UI_METHODS and not blocked_by_assign)
            if in_ui and content and not has_cjk(content):
                visible, _ = split_id(content)
                if visible and visible not in SKIP_STRINGS and DICT.get(visible) is None:
                    line = text.count("\n", 0, pos) + 1
                    misses.append((line, content))
        pos += len(tok)

    new_text = "".join(out)
    return text, new_text, changed, misses, applied, bom


def main():
    report_only = "--report" in sys.argv
    files_changed = 0
    total_changed = 0
    all_misses = []
    all_applied = []
    show_list = "--list" in sys.argv

    for root, dirs, fns in os.walk(SRC):
        dirs[:] = [d for d in dirs if d not in (".git", "bin", "obj")]
        for fn in sorted(fns):
            if not fn.endswith(".cs"):
                continue
            path = os.path.join(root, fn)
            rel = os.path.relpath(path, ROOT)
            if os.path.normpath(rel) in SKIP_FILES:
                continue
            text, new_text, changed, misses, applied, bom = process_file(path)
            if changed:
                files_changed += 1
                total_changed += changed
                if not report_only:
                    data = new_text.encode("utf-8")
                    if bom:
                        data = b"\xef\xbb\xbf" + data
                    open(path, "wb").write(data)
            all_misses.extend((rel, l, s) for l, s in misses)
            all_applied.extend((rel, l, s, t) for l, s, t in applied)

    print(f"translated strings: {total_changed}, files: {files_changed}")
    if show_list:
        print("--- translations that would be applied ---")
        for rel, line, s, t in all_applied:
            print(f"  {rel}:{line}  {s!r} -> {t!r}")
    print(f"UI-context strings not in dictionary: {len(all_misses)}")
    for rel, line, s in all_misses[:120]:
        print(f"  {rel}:{line}  {s!r}")
    if len(all_misses) > 120:
        print(f"  ... and {len(all_misses) - 120} more")


if __name__ == "__main__":
    main()
