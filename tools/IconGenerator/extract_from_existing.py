#!/usr/bin/env python3
"""
Extract IconMetadata.json from existing IconKind.cs file
This preserves all manual translations and categorizations
"""

import json
import re
from pathlib import Path
from typing import List, Dict, Any


def extract_icon_data_from_cs(cs_file_path: str) -> List[Dict[str, Any]]:
    """
    Parse IconKind.cs and extract icon metadata.

    Args:
        cs_file_path: Path to IconKind.cs

    Returns:
        List of icon metadata dictionaries
    """
    with open(cs_file_path, 'r', encoding='utf-8') as f:
        content = f.read()

    icons = []
    current_category = "uncategorized"

    # Pattern to match IconData attribute and enum value
    # Example: [IconData("\uE72B", "Back", "导航")]
    #         Back,
    pattern = r'\[IconData\("([^"]+)",\s*"([^"]+)",\s*"([^"]+)"\)\]\s*(\w+),?'

    for match in re.finditer(pattern, content):
        unicode_string = match.group(1)  # \uE72B
        name = match.group(2)             # Back
        category = match.group(3)         # 导航
        enum_name = match.group(4)        # Back

        # Extract unicode hex (E72B from \uE72B)
        unicode_match = re.search(r'\\u([0-9A-Fa-f]{4})', unicode_string)
        unicode_hex = unicode_match.group(1).upper() if unicode_match else ""

        # Convert category to key
        category_map = {
            "导航": "navigation",
            "操作": "actions",
            "媒体": "media",
            "通信": "communication",
            "文件": "files",
            "状态": "status",
            "界面": "ui",
            "开发": "development",
            "安全": "security",
            "": "uncategorized"
        }
        category_key = category_map.get(category, "uncategorized")

        # Generate keywords
        keywords = [
            name.lower(),
            enum_name.lower(),
            category_key.lower()
        ]

        # Add common aliases/translations
        translations = {
            "Back": ["后退", "返回"],
            "Forward": ["前进", "下一步"],
            "Up": ["上", "向上"],
            "Down": ["下", "向下"],
            "Left": ["左", "向左"],
            "Right": ["右", "向右"],
            "Home": ["首页", "主页", "家"],
            "Refresh": ["刷新", "重新加载"],
            "Menu": ["菜单", "目录"],
            "Add": ["添加", "新增", "加号", "+"],
            "Delete": ["删除", "移除", "废纸篓"],
            "Edit": ["编辑", "修改"],
            "Save": ["保存", "存档"],
            "Open": ["打开", "开启"],
            "Close": ["关闭", "闭合"],
            "Cancel": ["取消", "撤销"],
            "OK": ["确定", "确认", "好"],
            "Yes": ["是"],
            "No": ["否"],
            "Search": ["搜索", "查找", "放大镜"],
            "Settings": ["设置", "配置", "选项"],
            "Help": ["帮助", "疑问"],
            "Print": ["打印", "打印机"],
            "View": ["查看", "视图"],
            "Copy": ["复制", "拷贝"],
            "Cut": ["剪切", "裁剪"],
            "Paste": ["粘贴"],
            "Undo": ["撤销", "取消操作"],
            "Redo": ["重做", "恢复操作"],
            "ZoomIn": ["放大"],
            "ZoomOut": ["缩小"],
            "FullS creen": ["全屏"],
            "Folder": ["文件夹", "目录"],
            "File": ["文件"],
            "Mail": ["邮件", "信封"],
            "Calendar": ["日历", "历法", "时间表"],
            "Contact": ["联系人", "通讯录"],
            "Phone": ["电话", "手机"],
            "Camera": ["相机", "拍照", "摄像头"],
            "Video": ["视频", "录像"],
            "Music": ["音乐", "歌曲"],
            "Volume": ["音量", "声音"],
            "Mute": ["静音", "消音"],
            "Wifi": ["无线", "Wi-Fi"],
            "Bluetooth": ["蓝牙"],
            "Battery": ["电池", "电量"],
            "Lock": ["锁定", "锁"],
            "Unlock": ["解锁", "开锁"],
            "Shield": ["盾牌", "安全", "防护"],
            "Key": ["密钥", "钥匙", "关键"],
            "Certificate": ["证书", "凭证"],
            "Admin": ["管理员", "管理"],
            "User": ["用户", "账号"],
            "Group": ["组", "群组"],
            "Team": ["团队", "队伍"],
            "Flag": ["旗帜", "标记"],
            "Tag": ["标签", "标记"],
            "Like": ["点赞", "喜欢"],
            "Dislike": ["点踩", "不喜欢"],
            "Star": ["星", "收藏", "评分"],
            "Heart": ["心", "喜爱"],
            "Comment": ["评论", "留言"],
            "Share": ["分享"],
            "Upload": ["上传"],
            "Download": ["下载"],
            "Cloud": ["云", "云端"],
            "Sync": ["同步"],
            "Error": ["错误", "异常", "警告"],
            "Warning": ["警告", "注意"],
            "Info": ["信息", "提示"],
            "Success": ["成功", "完成"],
            "Loading": ["加载", "等待"],
            "Spinner": ["旋转", "加载中"],
        }

        if name in translations:
            keywords.extend(translations[name])

        icon = {
            "glyph": f"u{unicode_hex}",
            "unicode": unicode_hex,
            "unicode_string": unicode_string,
            "name": name,
            "enum_name": enum_name,
            "category": category_key,
            "keywords": list(set(keywords)),  # Remove duplicates
            "i18n": {
                "en": name,
                "zh": category
            }
        }

        icons.append(icon)

    return icons


def main():
    """Main entry point."""
    project_root = Path(__file__).parent.parent.parent
    iconkind_path = project_root / "src" / "UI" / "Lemoo.UI" / "Models" / "Icons" / "IconKind.cs"
    output_path = Path(__file__).parent / "IconMetadata.json"

    print("=" * 60)
    print("Extract Icon Metadata from Existing IconKind.cs")
    print("=" * 60)

    if not iconkind_path.exists():
        print(f"Error: IconKind.cs not found at {iconkind_path}")
        return 1

    # Extract icons
    icons = extract_icon_data_from_cs(str(iconkind_path))
    print(f"Extracted {len(icons)} icons from IconKind.cs")

    # Build metadata
    metadata = {
        "$schema": "./IconMetadata.schema.json",
        "font": {
            "name": "Segoe Fluent Icons",
            "version": "1.00",
            "copyright": "© 2021 Microsoft Corporation. All Rights Reserved."
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

    # Save to JSON
    with open(output_path, 'w', encoding='utf-8') as f:
        json.dump(metadata, f, indent=2, ensure_ascii=False)

    print(f"Saved to: {output_path}")

    # Statistics
    from collections import Counter
    categories = Counter([i['category'] for i in icons])

    print("\nCategory breakdown:")
    for cat, count in sorted(categories.items(), key=lambda x: x[1], reverse=True):
        cat_name = next((c['name_zh'] for c in metadata['categories'] if c['key'] == cat), cat)
        print(f"  {cat_name:12s}: {count:4d} icons")

    print("\n" + "=" * 60)
    print("Extraction complete!")
    print("=" * 60)

    return 0


if __name__ == "__main__":
    exit(main())
