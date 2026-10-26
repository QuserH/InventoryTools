# -*- coding: utf-8 -*-
"""
One-time migration: inline LocalizationService.Ui("English") / Format("English", args)
into source-level Chinese using tools/zh-CN.json (AutoRetainer-style).

- Ui("literal")            -> "中文"   (dictionary miss -> original literal)
- Ui(Ui("literal"))        -> "中文"
- Ui(dynamic expression)   -> left untouched (runtime translation still available)
- Format("literal", args)  -> string.Format("中文", args)
- Format(Ui("literal"), args) -> string.Format("中文", args)
- [HelpMessage("...")]     -> translated literal
- ## / ### ImGui ids are preserved (only the visible part is translated)
- Lists/ListImportExportService.cs is skipped (protocol tokens must stay raw English)
"""
import io, json, os, re, sys

DRY = "--dry" in sys.argv
args = [a for a in sys.argv[1:] if not a.startswith("--")]
repo = args[0]
src_root = os.path.join(repo, "InventoryTools")
dict_path = os.path.join(repo, "tools", "zh-CN.json")
SKIP_FILES = {
    os.path.normpath(os.path.join("InventoryTools", "Localization", "LocalizationService.cs")),
    os.path.normpath(os.path.join("InventoryTools", "Localization", "UiTranslations.cs")),
}
KEEP_FILES = {
    os.path.normpath(os.path.join("InventoryTools", "Lists", "ListImportExportService.cs")),
}

with io.open(dict_path, encoding="utf-8-sig") as f:
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

def translate_text(s):
    if not s or s == "":
        return s
    visible, ident = split_id(s)
    if not visible:
        return s
    hit = DICT.get(visible)
    if hit is None:
        return s
    return hit + ident

def unescape_cs(s):
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

def escape_cs(s):
    return (s.replace("\\", "\\\\").replace('"', '\\"')
             .replace("\n", "\\n").replace("\r", "\\r").replace("\t", "\\t"))

def literal_text(s):
    """Return (content, is_literal) for a C# string literal expression."""
    s = s.strip()
    if len(s) < 2 or not s.startswith('"') or not s.endswith('"'):
        return None
    # must be a single literal (no concatenation)
    j = 1
    while j < len(s):
        if s[j] == "\\":
            j += 2; continue
        if s[j] == '"':
            if j != len(s) - 1:
                return None
            return unescape_cs(s[1:-1])
        j += 1
    return None

def find_call_end(text, open_paren_idx):
    depth = 1
    i = open_paren_idx + 1
    n = len(text)
    while i < n:
        c = text[i]
        if c == '"':
            i += 1
            while i < n:
                if text[i] == "\\":
                    i += 2; continue
                if text[i] == '"':
                    i += 1; break
                i += 1
            continue
        if c == "'":
            i += 1
            while i < n:
                if text[i] == "\\":
                    i += 2; continue
                if text[i] == "'":
                    i += 1; break
                i += 1
            continue
        if c in "([{":
            depth += 1
        elif c in ")]}":
            depth -= 1
            if depth == 0:
                return i
        i += 1
    return n - 1

def split_args(text, open_paren_idx, close_idx):
    args = []
    depth = 1
    i = open_paren_idx + 1
    cur = i
    while i < close_idx:
        c = text[i]
        if c == '"':
            i += 1
            while i < close_idx:
                if text[i] == "\\":
                    i += 2; continue
                if text[i] == '"':
                    i += 1; break
                i += 1
            continue
        if c == "'":
            i += 1
            while i < close_idx:
                if text[i] == "\\":
                    i += 2; continue
                if text[i] == "'":
                    i += 1; break
                i += 1
            continue
        if c in "([{":
            depth += 1
        elif c in ")]}":
            depth -= 1
        elif c == "," and depth == 1:
            args.append(text[cur:i]); cur = i + 1
        i += 1
    args.append(text[cur:close_idx])
    return args

def resolve_ui_arg(arg):
    """Return (replacement_text, hit, source) or (None, None, None) to keep runtime call.
    hit: True if translated / already Chinese / nothing to translate,
         False if plain English literal missing from the dictionary."""
    a = arg.strip()
    if a == "":
        return None, None, None
    content = literal_text(a)
    if content is not None:
        if content == "":
            return '""', True, content
        if has_cjk(content):
            return '"' + escape_cs(content) + '"', True, content
        visible, _ = split_id(content)
        if not visible:
            return '"' + escape_cs(content) + '"', True, content
        translated = translate_text(content)
        return '"' + escape_cs(translated) + '"', translated != content, content
    # nested Ui(...)
    if a.startswith("LocalizationService.Ui") and "(" in a:
        op = a.index("(")
        close = find_call_end(a, op)
        if a[close + 1:].strip() == "":
            inner_args = split_args(a, op, close)
            if len(inner_args) == 1:
                rep, hit, source = resolve_ui_arg(inner_args[0])
                if rep is not None:
                    return rep, hit, source
                return "LocalizationService.Ui(" + inner_args[0].strip() + ")", None, None
        return None, None, None
    return None, None, None

def resolve_format_first(arg):
    a = arg.strip()
    content = literal_text(a)
    if content is not None:
        if has_cjk(content):
            return '"' + escape_cs(content) + '"'
        return '"' + escape_cs(translate_text(content)) + '"'
    if a.startswith("LocalizationService.Ui"):
        op = a.index("(")
        close = find_call_end(a, op)
        if a[close + 1:].strip() == "":
            inner_args = split_args(a, op, close)
            if len(inner_args) == 1:
                rep, _, _ = resolve_ui_arg(inner_args[0])
                return rep
    return None

def process(text):
    out = []
    i = 0
    n = len(text)
    stats = {"ui": 0, "ui_inline": 0, "ui_keep": 0, "fmt": 0, "fmt_inline": 0,
             "miss": [], "kept": []}
    while i < n:
        # comments
        if text.startswith("//", i):
            j = text.find("\n", i)
            j = n if j < 0 else j
            out.append(text[i:j]); i = j; continue
        if text.startswith("/*", i):
            j = text.find("*/", i + 2)
            j = n if j < 0 else j + 2
            out.append(text[i:j]); i = j; continue
        # strings / chars
        if text[i] == '"':
            j = i + 1
            while j < n:
                if text[j] == "\\":
                    j += 2; continue
                if text[j] == '"':
                    j += 1; break
                j += 1
            out.append(text[i:j]); i = j; continue
        if text[i] == "'":
            j = i + 1
            while j < n:
                if text[j] == "\\":
                    j += 2; continue
                if text[j] == "'":
                    j += 1; break
                j += 1
            out.append(text[i:j]); i = j; continue
        # call sites
        for name in ("LocalizationService.Ui", "LocalizationService.Format"):
            if text.startswith(name, i):
                k = i + len(name)
                while k < n and text[k] in " \t\r\n":
                    k += 1
                if k < n and text[k] == "(":
                    close = find_call_end(text, k)
                    args = split_args(text, k, close)
                    original = text[i:close + 1]
                    line = text.count("\n", 0, i) + 1
                    if name.endswith(".Ui"):
                        stats["ui"] += 1
                        if len(args) == 1:
                            rep, hit, source = resolve_ui_arg(args[0])
                            if rep is not None:
                                out.append(rep); i = close + 1
                                stats["ui_inline"] += 1
                                if hit is False:
                                    stats["miss"].append((line, (source or "")[:80]))
                                break
                        stats["ui_keep"] += 1
                        stats["kept"].append((line, original[:100].replace("\n", " ")))
                        out.append(original); i = close + 1
                        break
                    else:
                        stats["fmt"] += 1
                        if len(args) >= 1:
                            rep = resolve_format_first(args[0])
                            if rep is not None:
                                rest = ",".join(args[1:]) if len(args) > 1 else ""
                                out.append("string.Format(" + rep + ("," + rest if rest else "") + ")")
                                i = close + 1
                                stats["fmt_inline"] += 1
                                break
                        out.append(original); i = close + 1
                        break
        else:
            out.append(text[i]); i += 1
    return "".join(out), stats

HELP = re.compile(r'(\[HelpMessage\(\s*")((?:[^"\\]|\\.)*)(")')

changed = []
total = {"ui": 0, "ui_inline": 0, "ui_keep": 0, "fmt": 0, "fmt_inline": 0}
all_miss = []
all_kept = []

for root, dirs, fns in os.walk(src_root):
    dirs[:] = [d for d in dirs if d not in ("bin", "obj", ".git")]
    for fn in sorted(fns):
        if not fn.endswith(".cs"):
            continue
        path = os.path.join(root, fn)
        rel = os.path.relpath(path, repo)
        nrel = os.path.normpath(rel)
        if nrel in SKIP_FILES:
            continue
        raw = open(path, "rb").read()
        bom = raw.startswith(b"\xef\xbb\xbf")
        text = raw.decode("utf-8-sig")
        newline = "\r\n" if "\r\n" in text else "\n"

        if nrel in KEEP_FILES:
            continue

        new, stats = process(text)
        # HelpMessage attribute literals
        def repl_help(m):
            content = unescape_cs(m.group(2))
            if has_cjk(content):
                return m.group(0)
            t = translate_text(content)
            return m.group(1) + escape_cs(t) + m.group(3)
        new = HELP.sub(repl_help, new)

        if new != text:
            if not DRY:
                encoded = new.encode("utf-8")
                if bom:
                    encoded = b"\xef\xbb\xbf" + encoded
                open(path, "wb").write(encoded)
            changed.append(rel)

        for k in ("ui", "ui_inline", "ui_keep", "fmt", "fmt_inline"):
            total[k] += stats[k]
        all_miss.extend((rel, l, s) for l, s in stats["miss"])
        all_kept.extend((rel, l, s) for l, s in stats["kept"])

print("stats:", total)
print("changed files:", len(changed))
print("\n--- Ui/Format calls kept (runtime) ---")
for rel, l, s in all_kept:
    print(f"  {rel}:{l}  {s}")
print("\n--- dictionary misses among literal Ui (kept as English) ---")
for rel, l, s in all_miss[:80]:
    print(f"  {rel}:{l}  {s!r}")
print("miss count:", len(all_miss))
