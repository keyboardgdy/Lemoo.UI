#!/usr/bin/env python3
"""Check what glyphs are actually in the font file."""

from fontTools.ttLib import TTFont

font_path = r"D:\Code\Claude\Lemoo.UI\src\UI\Lemoo.UI\Resources\Fonts\Segoe Fluent Icons.ttf"
font = TTFont(font_path)
cmap = font['cmap'].getBestCmap()

print("Checking if key icons exist at their Unicode values:")
for code, name in [(0xE72A, 'Forward'), (0xE72B, 'Back'), (0xE721, 'Search'), (0xE80F, 'Home'), (0xE713, 'Settings')]:
    result = cmap.get(code)
    print(f"{hex(code)} ({name}): {result}")

print("\nSample of 30 glyphs from E700-E900 range:")
count = 0
for code, name in sorted(cmap.items()):
    if 0xE700 <= code <= 0xE900 and count < 30:
        print(f"{hex(code)}: {name}")
        count += 1

print(f"\nTotal glyphs in cmap: {len(cmap)}")
print(f"Glyphs in E700-F800 range: {sum(1 for c in cmap.keys() if 0xE700 <= c <= 0xF800)}")
