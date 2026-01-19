#!/usr/bin/env python3
"""Show icon range coverage."""

import json

metadata_path = r"D:\Code\Claude\Lemoo.UI\src\UI\Lemoo.UI\Models\Icons\IconMetadata.json"
with open(metadata_path, 'r', encoding='utf-8') as f:
    data = json.load(f)

icons = data['icons']

ranges = [
    ('E700-E900', 0xE700, 0xE900),
    ('EA00-EC00', 0xEA00, 0xEC00),
    ('ED00-EF00', 0xED00, 0xEF00),
    ('F000-F200', 0xF000, 0xF200),
    ('F300-F500', 0xF300, 0xF500),
    ('F600-F800', 0xF600, 0xF800)
]

print("=" * 80)
print("ICON RANGE COVERAGE")
print("=" * 80)
print(f"{'Range':<15} {'Icons':>10}")
print("-" * 80)

total = 0
for name, start, end in ranges:
    count = sum(1 for i in icons if start <= int(i['unicode'], 16) < end)
    total += count
    print(f"{name:<15} {count:>10}")

print("-" * 80)
print(f"{'TOTAL':<15} {total:>10}")
print("=" * 80)
