#!/usr/bin/env python3
"""Show statistics of generated metadata."""

import json

metadata_path = r"D:\Code\Claude\Lemoo.UI\src\UI\Lemoo.UI\Models\Icons\IconMetadata.json"
with open(metadata_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

print("=" * 80)
print("IconMetadata.json STATISTICS")
print("=" * 80)

print(f"\nTotal icons: {len(data['icons'])}")
print(f"Total categories: {len(data['categories'])}")

print("\nCategories breakdown:")
for cat in sorted(data['categories'], key=lambda x: x['priority']):
    count = sum(1 for i in data['icons'] if i['category'] == cat['key'])
    print(f"  - {cat['name']:20s} ({cat['name_zh']}): {count:4d} icons")

print("\n" + "=" * 80)
