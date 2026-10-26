# -*- coding: utf-8 -*-
"""
InventoryTools 汉化结构性补丁（AutoRetainer-zh 同款流程）

localize.py 只能处理「UI 调用参数 / 文本属性 / HelpMessage」这些上下文。
有些展示文本位于 return、switch、字典初始化等位置，用本脚本按英文原文重放。

上游同步后顺序执行：
    python tools/localize.py
    python tools/patch_cn.py

匹配原则：
- `old` 为英文原文（或 localize 之后的中文），`new` 为替换结果。
- 已经包含 `new` 视为已应用（幂等）。
- 两边都找不到 → 报错并以非零退出（说明上游改动过此处，需要人工处理）。
"""
import io
import os
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))

# (相对路径, 原文, 译文)
PATCHES = [
    # 空物品在提示里显示为「空」
    ("InventoryTools/Localizers/ItemLocalizer.cs",
     'return "Empty";',
     'return "空";'),
]


def apply(patch, dry=False):
    rel, old, new = patch
    path = os.path.join(ROOT, rel.replace("/", os.sep))
    if not os.path.exists(path):
        return "missing-file", None
    with io.open(path, encoding="utf-8-sig", errors="replace", newline="") as f:
        text = f.read()
    if new in text:
        return "already", None
    if old not in text:
        return "no-match", None
    if not dry:
        bom = open(path, "rb").read(3) == b"\xef\xbb\xbf"
        text = text.replace(old, new)
        data = text.encode("utf-8")
        if bom:
            data = b"\xef\xbb\xbf" + data
        open(path, "wb").write(data)
    return "applied", None


def main():
    dry = "--check" in sys.argv
    ok = 0
    failed = []
    for patch in PATCHES:
        status, _ = apply(patch, dry=dry)
        if status in ("applied", "already"):
            ok += 1
        else:
            failed.append((patch, status))
    print(f"patch_cn: {ok}/{len(PATCHES)} applied or already present")
    if failed:
        print("以下补丁需要人工处理：")
        for patch, status in failed:
            print(f"  - {patch[0]}: {status} ({patch[1]!r})")
        sys.exit(1)


if __name__ == "__main__":
    main()
