#!/usr/bin/env python3
"""
Segoe Fluent Icons Font Parser

This script extracts icon metadata from Segoe Fluent Icons.ttf file
and generates IconMetadata.json for code generation.

Requirements:
    pip install fonttools

Usage:
    python parse_font.py
"""

import json
import re
from pathlib import Path
from fontTools.ttLib import TTFont
from typing import List, Dict, Any


class IconMetadataExtractor:
    """Extract icon metadata from TrueType font files."""

    def __init__(self, font_path: str):
        """
        Initialize the extractor.

        Args:
            font_path: Path to the .ttf font file
        """
        self.font_path = Path(font_path)
        self.font = None
        self.icons = []

    def load_font(self) -> None:
        """Load the font file."""
        if not self.font_path.exists():
            raise FileNotFoundError(f"Font file not found: {self.font_path}")

        print(f"Loading font: {self.font_path}")
        self.font = TTFont(str(self.font_path))
        print(f"Font loaded successfully: {self.font['name'].getBestFullName()}")

    def get_glyph_names(self) -> List[str]:
        """
        Get all glyph names from the font.

        Returns:
            List of glyph names
        """
        if not self.font:
            raise RuntimeError("Font not loaded. Call load_font() first.")

        # Get the glyph order (exclude .notdef and special glyphs)
        glyph_order = self.font.getGlyphOrder()
        return [name for name in glyph_order if not name.startswith('.')]

    def extract_unicode_from_glyph_name(self, glyph_name: str) -> str:
        """
        Extract Unicode code point from glyph name.

        Segoe Fluent Icons uses naming convention like:
        - 'uE72B' -> U+E72B
        - 'uniE72B' -> U+E72B
        - 'E72B' -> U+E72B

        Args:
            glyph_name: The glyph name from font

        Returns:
            Unicode code point string (e.g., 'E72B')
        """
        # Remove common prefixes
        clean_name = glyph_name
        for prefix in ['u', 'uni', '_']:
            if clean_name.startswith(prefix):
                clean_name = clean_name[len(prefix):]
                break

        # Extract hex code (4-5 digit hex number)
        match = re.search(r'[0-9A-Fa-f]{4,5}', clean_name)
        if match:
            return match.group(0).upper()

        return None

    def estimate_category_from_glyph_name(self, glyph_name: str, unicode_point: str) -> str:
        """
        Estimate icon category based on glyph name and Unicode range.

        Segoe Fluent Icons Unicode ranges (approximate):
        - E700-E77F: Navigation & Basic
        - E780-E7FF: Actions & Editing
        - E800-E87F: Media & Audio
        - E880-E8FF: Communication
        - E900-E97F: Files & Folders
        - E980-E9FF: Status & Alerts
        - EA00-EA7F: UI Elements
        - EA80-EAFF: Developer Tools
        - EB00-EBFF: Security & Privacy

        Args:
            glyph_name: The glyph name
            unicode_point: Unicode code point (e.g., 'E72B')

        Returns:
            Category name
        """
        # Convert hex to int for range comparison
        try:
            code = int(unicode_point, 16)
        except ValueError:
            return "uncategorized"

        # Categorize by Unicode range
        if 0xE700 <= code < 0xE780:
            return "navigation"
        elif 0xE780 <= code < 0xE800:
            return "actions"
        elif 0xE800 <= code < 0xE880:
            return "media"
        elif 0xE880 <= code < 0xE900:
            return "communication"
        elif 0xE900 <= code < 0xE980:
            return "files"
        elif 0xE980 <= code < 0xEA00:
            return "status"
        elif 0xEA00 <= code < 0xEA80:
            return "ui"
        elif 0xEA80 <= code < 0xEB00:
            return "development"
        elif 0xEB00 <= code < 0xEC00:
            return "security"
        else:
            return "uncategorized"

    def guess_icon_name_from_glyph(self, glyph_name: str) -> str:
        """
        Guess a human-readable icon name from glyph name.

        Examples:
            'uE72B' -> 'Back'
            'uniE80F' -> 'Home'
            'addinapp' -> 'AddInApp'

        Args:
            glyph_name: The glyph name from font

        Returns:
            Guessed icon name in PascalCase
        """
        # Remove common prefixes
        clean_name = glyph_name
        for prefix in ['u', 'uni', '_']:
            if clean_name.startswith(prefix):
                clean_name = clean_name[len(prefix):]
                break

        # If it's a pure hex code, we can't guess the name
        if re.match(r'^[0-9A-Fa-f]+$', clean_name):
            # Use a generic name based on Unicode
            return f"Icon_{clean_name.upper()}"

        # Convert to PascalCase
        # Replace separators with spaces, capitalize each word
        name = re.sub(r'[_\-\s]+', ' ', clean_name)
        name = ''.join(word.capitalize() for word in name.split())

        return name

    def extract_all_icons(self) -> List[Dict[str, Any]]:
        """
        Extract all icons from the font.

        Returns:
            List of icon metadata dictionaries
        """
        if not self.font:
            self.load_font()

        print("Extracting icon metadata...")

        # Get all glyph names
        glyph_names = self.get_glyph_names()
        print(f"Found {len(glyph_names)} glyphs")

        icons = []
        skipped = 0

        for glyph_name in glyph_names:
            # Extract Unicode
            unicode_hex = self.extract_unicode_from_glyph_name(glyph_name)
            if not unicode_hex:
                skipped += 1
                continue

            # Build icon metadata
            icon = {
                "glyph": glyph_name,
                "unicode": unicode_hex,
                "unicode_string": f"\\u{unicode_hex.lower()}",
                "name": self.guess_icon_name_from_glyph(glyph_name),
                "category": self.estimate_category_from_glyph_name(glyph_name, unicode_hex),
                "keywords": self._generate_keywords(glyph_name, unicode_hex),
                "i18n": {
                    "en": self.guess_icon_name_from_glyph(glyph_name),
                    "zh": ""  # To be filled manually
                }
            }

            icons.append(icon)

        print(f"Extracted {len(icons)} icons (skipped {skipped})")
        return icons

    def _generate_keywords(self, glyph_name: str, unicode_hex: str) -> List[str]:
        """
        Generate search keywords for an icon.

        Args:
            glyph_name: The glyph name
            unicode_hex: Unicode code point

        Returns:
            List of keywords
        """
        keywords = []

        # Add the glyph name
        keywords.append(glyph_name.lower())

        # Add Unicode
        keywords.append(unicode_hex.lower())
        keywords.append(f"u{unicode_hex.lower()}")

        # Add guessed name variations
        name = self.guess_icon_name_from_glyph(glyph_name)
        # Split PascalCase into words
        words = re.findall(r'[A-Z]?[a-z]+|[A-Z]+(?=[A-Z]|$)', name)
        keywords.extend([w.lower() for w in words])

        # Add category as keyword
        category = self.estimate_category_from_glyph_name(glyph_name, unicode_hex)
        keywords.append(category.lower())

        return list(set(keywords))  # Remove duplicates

    def save_to_json(self, output_path: str, icons: List[Dict[str, Any]]) -> None:
        """
        Save extracted icons to JSON file.

        Args:
            output_path: Path to output JSON file
            icons: List of icon metadata
        """
        output = Path(output_path)
        output.parent.mkdir(parents=True, exist_ok=True)

        metadata = {
            "$schema": "./IconMetadata.schema.json",
            "font": {
                "name": self.font['name'].getBestFullName() if self.font else "Unknown",
                "version": str(float(self.font['head'].fontRevision)) if self.font else "1.0",
                "copyright": self.font['name'].getDebugName(0) if self.font else ""
            },
            "categories": [
                {"key": "navigation", "name": "Navigation", "name_zh": "导航", "priority": 1},
                {"key": "actions", "name": "Actions", "name_zh": "操作", "priority": 2},
                {"key": "media", "name": "Media", "name_zh": "媒体", "priority": 3},
                {"key": "communication", "name": "Communication", "name_zh": "通信", "priority": 4},
                {"key": "files", "name": "Files", "name_zh": "文件", "priority": 5},
                {"key": "status", "name": "Status", "name_zh": "状态", "priority": 6},
                {"key": "ui", "name": "UI Elements", "name_zh": "界面", "priority": 7},
                {"key": "development", "name": "Development", "name_zh": "开发", "priority": 8},
                {"key": "security", "name": "Security", "name_zh": "安全", "priority": 9},
                {"key": "uncategorized", "name": "Uncategorized", "name_zh": "未分类", "priority": 999}
            ],
            "icons": icons
        }

        with open(output, 'w', encoding='utf-8') as f:
            json.dump(metadata, f, indent=2, ensure_ascii=False)

        print(f"Saved {len(icons)} icons to: {output}")

    def close(self) -> None:
        """Close the font file."""
        if self.font:
            self.font.close()


def main():
    """Main entry point."""
    # Paths
    project_root = Path(__file__).parent.parent.parent
    font_path = project_root / "src" / "UI" / "Lemoo.UI" / "Resources" / "Fonts" / "Segoe Fluent Icons.ttf"
    output_path = Path(__file__).parent / "IconMetadata.json"

    print("=" * 60)
    print("Segoe Fluent Icons Font Parser")
    print("=" * 60)

    # Extract metadata
    extractor = IconMetadataExtractor(str(font_path))

    try:
        icons = extractor.extract_all_icons()
        extractor.save_to_json(str(output_path), icons)

        print("\n" + "=" * 60)
        print("✓ Extraction complete!")
        print(f"✓ Output: {output_path}")
        print("=" * 60)

        # Print statistics
        categories = {}
        for icon in icons:
            cat = icon['category']
            categories[cat] = categories.get(cat, 0) + 1

        print("\nCategory breakdown:")
        for cat, count in sorted(categories.items(), key=lambda x: x[1], reverse=True):
            print(f"  {cat:20s}: {count:4d} icons")

    except Exception as e:
        print(f"\n✗ Error: {e}")
        import traceback
        traceback.print_exc()
        return 1

    finally:
        extractor.close()

    return 0


if __name__ == "__main__":
    exit(main())
