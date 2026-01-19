#!/usr/bin/env python3
"""Verify key icons are in the generated metadata."""

import json

metadata_path = r"D:\Code\Claude\Lemoo.UI\src\UI\Lemoo.UI\Models\Icons\IconMetadata.json"
with open(metadata_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

icons = {i['unicode']: i for i in data['icons']}

print("=" * 80)
print("KEY ICONS VERIFICATION")
print("=" * 80)

print("\nKey icons you requested:")
for code, name in [('E72B', 'Back'), ('E72A', 'Forward'), ('E721', 'Search'), ('E80F', 'Home'), ('E713', 'Settings')]:
    if code in icons:
        icon = icons[code]
        print(f"  {name:15s} ({code}): FOUND")
    else:
        print(f"  {name:15s} ({code}): MISSING")

print(f"\nTotal icons generated: {len(data['icons'])}")
print(f"Categories: {len(data['categories'])}")

print("\n" + "=" * 80)
print("SAMPLE ICON DETAILS")
print("=" * 80)

for code in ['E72B', 'E72A', 'E721', 'E80F', 'E713']:
    if code in icons:
        icon = icons[code]
        print(f"\n{icon['name']} ({code}):")
        print(f"  Category: {icon['category']}")
        print(f"  Chinese: {icon['i18n']['zh'] or 'N/A'}")
        print(f"  Keywords: {', '.join(icon['keywords'][:8])}")

print("\n" + "=" * 80)
