#!/usr/bin/env python3
"""Final verification of generated IconMetadata.json"""

import json

metadata_path = r"D:\Code\Claude\Lemoo.UI\src\UI\Lemoo.UI\Models\Icons\IconMetadata.json"
with open(metadata_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

icons_by_unicode = {i['unicode']: i['name'] for i in data['icons']}

print("=" * 80)
print("FINAL VERIFICATION - KEY ICONS")
print("=" * 80)
print(f"{'Unicode':<8} {'Expected Name':<20} {'Actual Name':<20} {'Status':<10}")
print("-" * 80)

key_icons = [
    ('E72A', 'Forward'),
    ('E72B', 'Back'),
    ('E721', 'Search'),
    ('E80F', 'Home'),
    ('E713', 'Settings')
]

all_ok = True
for code, expected_name in key_icons:
    actual_name = icons_by_unicode.get(code, 'NOT FOUND')
    status = 'OK' if actual_name == expected_name else 'MISMATCH'
    if status != 'OK':
        all_ok = False
    print(f"{code:<8} {expected_name:<20} {actual_name:<20} {status:<10}")

print("-" * 80)
print(f"Total icons in file: {len(data['icons'])}")
print(f"All key icons verified: {'YES' if all_ok else 'NO'}")
print("=" * 80)
