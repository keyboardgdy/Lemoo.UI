#!/usr/bin/env python3
"""
Create IconMetadata.json from Microsoft Official Documentation

This script:
1. Parses Microsoft's official Segoe Fluent Icons documentation
2. Extracts icon mappings (Unicode → Name)
3. Validates against the actual font file
4. Generates complete IconMetadata.json with official semantic names

Requirements:
    pip install fonttools requests beautifulsoup4

Usage:
    python create_official_metadata.py
"""

import json
import re
from pathlib import Path
from typing import List, Dict, Any, Set, Tuple
from fontTools.ttLib import TTFont


class OfficialIconMetadataGenerator:
    """Generate icon metadata from Microsoft official documentation."""

    # Microsoft documentation content (parsed from web)
    # This includes all icon ranges from the official documentation
    DOCUMENTATION_CONTENT = """
    This article provides developer guidelines for using the Segoe Fluent Icons font and lists each icon along with its Unicode value and descriptive name.

    ### PUA E700-E900

    | Glyph | Unicode point | Description |
    | --- | --- | --- |
    | e700 | GlobalNavButton |
    | e701 | Wifi |
    | e702 | Bluetooth |
    | e703 | Connect |
    | e704 | InternetSharing |
    | e705 | VPN |
    | e706 | Brightness |
    | e707 | MapPin |
    | e708 | QuietHours |
    | e709 | Airplane |
    | e70a | Tablet |
    | e70b | QuickNote |
    | e70c | RememberedDevice |
    | e70d | ChevronDown |
    | e70e | ChevronUp |
    | e70f | Edit |
    | e710 | Add |
    | e711 | Cancel |
    | e712 | More |
    | e713 | Settings |
    | e714 | Video |
    | e715 | Mail |
    | e716 | People |
    | e717 | Phone |
    | e718 | Pin |
    | e719 | Shop |
    | e71a | Stop |
    | e71b | Link |
    | e71c | Filter |
    | e71d | AllApps |
    | e71e | Zoom |
    | e71f | ZoomOut |
    | e720 | Microphone |
    | e721 | Search |
    | e722 | Camera |
    | e723 | Attach |
    | e724 | Send |
    | e725 | SendFill |
    | e726 | WalkSolid |
    | e727 | InPrivate |
    | e728 | FavoriteList |
    | e729 | PageSolid |
    | e72a | Forward |
    | e72b | Back |
    | e72c | Refresh |
    | e72d | Share |
    | e72e | Lock |
    | e72f | BlockedSite |
    | e730 | ReportHacked |
    | e731 | EMI |
    | e733 | Blocked |
    | e734 | FavoriteStar |
    | e735 | FavoriteStarFill |
    | e736 | ReadingMode |
    | e737 | Favicon |
    | e738 | Remove |
    | e739 | Checkbox |
    | e73a | CheckboxComposite |
    | e73b | CheckboxFill |
    | e73c | CheckboxIndeterminate |
    | e73d | CheckboxCompositeReversed |
    | e73e | CheckMark |
    | e73f | BackToWindow |
    | e740 | FullScreen |
    | e741 | ResizeTouchLarger |
    | e742 | ResizeTouchSmaller |
    | e743 | ResizeMouseSmall |
    | e744 | ResizeMouseMedium |
    | e745 | ResizeMouseWide |
    | e746 | ResizeMouseTall |
    | e747 | ResizeMouseLarge |
    | e748 | SwitchUser |
    | e749 | Print |
    | e74a | Up |
    | e74b | Down |
    | e74c | OEM |
    | e74d | Delete |
    | e74e | Save |
    | e74f | Mute |
    | e750 | BackSpaceQWERTY |
    | e751 | ReturnKey |
    | e752 | UpArrowShiftKey |
    | e753 | Cloud |
    | e754 | Flashlight |
    | e755 | RotationLock |
    | e756 | CommandPrompt |
    | e759 | SIPMove |
    | e75a | SIPUndock |
    | e75b | SIPRedock |
    | e75c | EraseTool |
    | e75d | UnderscoreSpace |
    | e75e | GripperTool |
    | e75f | Dialpad |
    | e760 | PageLeft |
    | e761 | PageRight |
    | e762 | MultiSelect |
    | e763 | KeyboardLeftHanded |
    | e764 | KeyboardRightHanded |
    | e765 | KeyboardClassic |
    | e766 | KeyboardSplit |
    | e767 | Volume |
    | e768 | Play |
    | e769 | Pause |
    | e76b | ChevronLeft |
    | e76c | ChevronRight |
    | e76d | InkingTool |
    | e76e | Emoji2 |
    | e76f | GripperBarHorizontal |
    | e770 | System |
    | e771 | Personalize |
    | e772 | Devices |
    | e773 | SearchAndApps |
    | e774 | Globe |
    | e775 | TimeLanguage |
    | e776 | EaseOfAccess |
    | e777 | UpdateRestore |
    | e778 | HangUp |
    | e779 | ContactInfo |
    | e77a | Unpin |
    | e77b | Contact |
    | e77c | Memo |
    | e77e | IncomingCall |
    | e77f | Paste |
    | e780 | PhoneBook |
    | e781 | LEDLight |
    | e783 | Error |
    | e784 | GripperBarVertical |
    | e785 | Unlock |
    | e786 | Slideshow |
    | e787 | Calendar |
    | e788 | GripperResize |
    | e789 | Megaphone |
    | e78a | Trim |
    | e78b | NewWindow |
    | e78c | SaveLocal |
    | e790 | Color |
    | e791 | DataSense |
    | e792 | SaveAs |
    | e793 | Light |
    | e794 | Effects |
    | e799 | AspectRatio |
    | e7a1 | Contrast |
    | e7a5 | DataSenseBar |
    | e7a6 | Redo |
    | e7a7 | Undo |
    | e7a8 | Crop |
    | e7aa | PhotoCollection |
    | e7ac | OpenWith |
    | e7ad | Rotate |
    | e7b3 | RedEye |
    | e7b5 | SetlockScreen |
    | e7b7 | MapPin2 |
    | e7b8 | Package |
    | e7ba | Warning |
    | e7bc | ReadingList |
    | e7be | Education |
    | e7bf | ShoppingCart |
    | e7c0 | Train |
    | e7c1 | Flag |
    | e7c2 | Move |
    | e7c3 | Page |
    | e7c4 | TaskView |
    | e7c5 | BrowsePhotos |
    | e7c6 | HalfStarLeft |
    | e7c7 | HalfStarRight |
    | e7c8 | Record |
    | e7c9 | TouchPointer |
    | e7de | LangJPN |
    | e7e3 | Ferry |
    | e7e6 | Highlight |
    | e7e7 | ActionCenterNotification |
    | e7e8 | PowerButton |
    | e7ea | ResizeTouchNarrower |
    | e7eb | ResizeTouchShorter |
    | e7ec | DrivingMode |
    | e7ed | RingerSilent |
    | e7ee | OtherUser |
    | e7ef | Admin |
    | e7f0 | CC |
    | e7f1 | SDCard |
    | e7f2 | CallForwarding |
    | e7f3 | SettingsDisplaySound |
    | e7f4 | TVMonitor |
    | e7f5 | Speakers |
    | e7f6 | Headphone |
    | e7f7 | DeviceLaptopPic |
    | e7f8 | DeviceLaptopNoPic |
    | e7f9 | DeviceMonitorRightPic |
    | e7fa | DeviceMonitorLeftPic |
    | e7fb | DeviceMonitorNoPic |
    | e7fc | Game |
    | e7fd | HorizontalTabKey |
    | e802 | StreetsideSplitMinimize |
    | e803 | StreetsideSplitExpand |
    | e804 | Car |
    | e805 | Walk |
    | e806 | Bus |
    | e809 | TiltUp |
    | e80a | TiltDown |
    | e80b | CallControl |
    | e80c | RotateMapRight |
    | e80d | RotateMapLeft |
    | e80f | Home |
    | e811 | ParkingLocation |
    | e812 | MapCompassTop |
    | e813 | MapCompassBottom |
    | e814 | IncidentTriangle |
    | e815 | Touch |
    | e816 | MapDirections |
    | e819 | StartPoint |
    | e81a | StopPoint |
    | e81b | EndPoint |
    | e81c | History |
    | e81d | Location |
    | e81e | MapLayers |
    | e81f | Accident |
    | e821 | Work |
    | e822 | Construction |
    | e823 | Recent |
    | e825 | Bank |
    | e826 | DownloadMap |
    | e829 | InkingToolFill2 |
    | e82a | HighlightFill2 |
    | e82b | EraseToolFill |
    | e82c | EraseToolFill2 |
    | e82d | Dictionary |
    | e82e | DictionaryAdd |
    | e82f | ToolTip |
    | e830 | ChromeBack |
    | e835 | ProvisioningPackage |
    | e836 | AddRemoteDevice |
    | e838 | FolderOpen |
    | e839 | Ethernet |
    | e83a | ShareBroadband |
    | e83b | DirectAccess |
    | e83c | DialUp |
    | e83d | DefenderApp |
    | e83e | BatteryCharging9 |
    | e83f | Battery10 |
    | e840 | Pinned |
    | e841 | PinFill |
    | e842 | PinnedFill |
    | e843 | PeriodKey |
    | e844 | PuncKey |
    | e845 | RevToggleKey |
    | e846 | RightArrowKeyTime1 |
    | e847 | RightArrowKeyTime2 |
    | e848 | LeftQuote |
    | e849 | RightQuote |
    | e84a | DownShiftKey |
    | e84b | UpShiftKey |
    | e84c | PuncKey0 |
    | e84d | PuncKeyLeftBottom |
    | e84e | RightArrowKeyTime3 |
    | e84f | RightArrowKeyTime4 |
    | e850 | Battery0 |
    | e851 | Battery1 |
    | e852 | Battery2 |
    | e853 | Battery3 |
    | e854 | Battery4 |
    | e855 | Battery5 |
    | e856 | Battery6 |
    | e857 | Battery7 |
    | e858 | Battery8 |
    | e859 | Battery9 |
    | e85a | BatteryCharging0 |
    | e85b | BatteryCharging1 |
    | e85c | BatteryCharging2 |
    | e85d | BatteryCharging3 |
    | e85e | BatteryCharging4 |
    | e85f | BatteryCharging5 |
    | e860 | BatteryCharging6 |
    | e861 | BatteryCharging7 |
    | e862 | BatteryCharging8 |
    | e863 | BatterySaver0 |
    | e864 | BatterySaver1 |
    | e865 | BatterySaver2 |
    | e866 | BatterySaver3 |
    | e867 | BatterySaver4 |
    | e868 | BatterySaver5 |
    | e869 | BatterySaver6 |
    | e86a | BatterySaver7 |
    | e86b | BatterySaver8 |
    | e86c | SignalBars1 |
    | e86d | SignalBars2 |
    | e86e | SignalBars3 |
    | e86f | SignalBars4 |
    | e870 | SignalBars5 |
    | e871 | SignalNotConnected |
    | e872 | Wifi1 |
    | e873 | Wifi2 |
    | e874 | Wifi3 |
    | e875 | MobSIMLock |
    | e876 | MobSIMMissing |
    | e877 | Vibrate |
    | e878 | RoamingInternational |
    | e879 | RoamingDomestic |
    | e87a | CallForwardInternational |
    | e87b | CallForwardRoaming |
    | e87c | JpnRomanji |
    | e87d | JpnRomanjiLock |
    | e87e | JpnRomanjiShift |
    | e87f | JpnRomanjiShiftLock |
    | e880 | StatusDataTransfer |
    | e881 | StatusDataTransferVPN |
    | e882 | StatusDualSIM2 |
    | e883 | StatusDualSIM2VPN |
    | e884 | StatusDualSIM1 |
    | e885 | StatusDualSIM1VPN |
    | e886 | StatusSGLTE |
    | e887 | StatusSGLTECell |
    | e888 | StatusSGLTEDataVPN |
    | e889 | StatusVPN |
    | e88a | WifiHotspot |
    | e88b | LanguageKor |
    | e88c | LanguageCht |
    | e88d | LanguageChs |
    | e88e | USB |
    | e88f | InkingToolFill |
    | e890 | View |
    | e891 | HighlightFill |
    | e892 | Previous |
    | e893 | Next |
    | e894 | Clear |
    | e895 | Sync |
    | e896 | Download |
    | e897 | Help |
    | e898 | Upload |
    | e899 | Emoji |
    | e89a | TwoPage |
    | e89b | LeaveChat |
    | e89c | MailForward |
    | e89e | RotateCamera |
    | e89f | ClosePane |
    | e8a0 | OpenPane |
    | e8a1 | PreviewLink |
    | e8a2 | AttachCamera |
    | e8a3 | ZoomIn |
    | e8a4 | Bookmarks |
    | e8a5 | Document |
    | e8a6 | ProtectedDocument |
    | e8a7 | OpenInNewWindow |
    | e8a8 | MailFill |
    | e8a9 | ViewAll |
    | e8aa | VideoChat |
    | e8ab | Switch |
    | e8ac | Rename |
    | e8ad | Go |
    | e8ae | SurfaceHub |
    | e8af | Remote |
    | e8b0 | Click |
    | e8b1 | Shuffle |
    | e8b2 | Movies |
    | e8b3 | SelectAll |
    | e8b4 | Orientation |
    | e8b5 | Import |
    | e8b6 | ImportAll |
    | e8b7 | Folder |
    | e8b8 | Webcam |
    | e8b9 | Picture |
    | e8ba | Caption |
    | e8bb | ChromeClose |
    | e8bc | ShowResults |
    | e8bd | Message |
    | e8be | Leaf |
    | e8bf | CalendarDay |
    | e8c0 | CalendarWeek |
    | e8c1 | Characters |
    | e8c2 | MailReplyAll |
    | e8c3 | Read |
    | e8c4 | ShowBcc |
    | e8c5 | HideBcc |
    | e8c6 | Cut |
    | e8c7 | PaymentCard |
    | e8c8 | Copy |
    | e8c9 | Important |
    | e8ca | MailReply |
    | e8cb | Sort |
    | e8cc | MobileTablet |
    | e8cd | DisconnectDrive |
    | e8ce | MapDrive |
    | e8cf | ContactPresence |
    | e8d0 | Priority |
    | e8d1 | GotoToday |
    | e8d2 | Font |
    | e8d3 | FontColor |
    | e8d4 | Contact2 |
    | e8d5 | FolderFill |
    | e8d6 | Audio |
    | e8d7 | Permissions |
    | e8d8 | DisableUpdates |
    | e8d9 | Unfavorite |
    | e8da | OpenLocal |
    | e8db | Italic |
    | e8dc | Underline |
    | e8dd | Bold |
    | e8de | MoveToFolder |
    | e8df | LikeDislike |
    | e8e0 | Dislike |
    | e8e1 | Like |
    | e8e2 | AlignRight |
    | e8e3 | AlignCenter |
    | e8e4 | AlignLeft |
    | e8e5 | OpenFile |
    | e8e6 | ClearSelection |
    | e8e7 | FontDecrease |
    | e8e8 | FontIncrease |
    | e8e9 | FontSize |
    | e8ea | CellPhone |
    | e8eb | Reshare |
    | e8ec | Tag |
    | e8ed | RepeatOne |
    | e8ee | RepeatAll |
    | e8ef | Calculator |
    | e8f0 | Directions |
    | e8f1 | Library |
    | e8f2 | ChatBubbles |
    | e8f3 | PostUpdate |
    | e8f4 | NewFolder |
    | e8f5 | CalendarReply |
    | e8f6 | UnsyncFolder |
    | e8f7 | SyncFolder |
    | e8f8 | BlockContact |
    | e8f9 | SwitchApps |
    | e8fa | AddFriend |
    | e8fb | Accept |
    | e8fc | GoToStart |
    | e8fd | BulletedList |
    | e8fe | Scan |
    | e8ff | Preview |
    | e902 | Group |
    | e904 | ZeroBars |
    | e905 | OneBar |
    | e906 | TwoBars |
    | e907 | ThreeBars |
    | e908 | FourBars |
    | e909 | World |
    | e90a | Comment |
    | e90b | MusicInfo |
    | e90c | DockLeft |
    | e90d | DockRight |
    | e90e | DockBottom |
    | e90f | Repair |
    | e910 | Accounts |
    | e911 | DullSound |
    | e912 | Manage |
    | e913 | Street |
    | e914 | Printer3D |
    | e915 | RadioBullet |
    | e916 | Stopwatch |
    | e91b | Photo |
    | e91c | ActionCenter |
    | e91f | FullCircleMask |
    | e921 | ChromeMinimize |
    | e922 | ChromeMaximize |
    | e923 | ChromeRestore |
    | e924 | Annotation |
    | e925 | BackSpaceQWERTYSm |
    | e926 | BackSpaceQWERTYMd |
    | e927 | Swipe |
    | e928 | Fingerprint |
    | e929 | Handwriting |
    | e92c | ChromeBackToWindow |
    | e92d | ChromeFullScreen |
    | e92e | KeyboardStandard |
    | e92f | KeyboardDismiss |
    | e930 | Completed |
    | e931 | ChromeAnnotate |
    | e932 | Label |
    | e933 | IBeam |
    | e934 | IBeamOutline |
    | e935 | FlickDown |
    | e936 | FlickUp |
    | e937 | FlickLeft |
    | e938 | FlickRight |
    | e939 | FeedbackApp |
    | e93c | MusicAlbum |
    | e93e | Streaming |
    | e943 | Code |
    | e944 | ReturnToWindow |
    | e945 | LightningBolt |
    | e946 | Info |
    | e947 | CalculatorMultiply |
    | e948 | CalculatorAddition |
    | e949 | CalculatorSubtract |
    | e94a | CalculatorDivide |
    | e94b | CalculatorSquareroot |
    | e94c | CalculatorPercentage |
    | e94d | CalculatorNegate |
    | e94e | CalculatorEqualTo |
    | e94f | CalculatorBackspace |
    | e950 | Component |
    | e951 | DMC |
    | e952 | Dock |
    | e953 | MultimediaDMS |
    | e954 | MultimediaDVR |
    | e955 | MultimediaPMP |
    | e956 | PrintfaxPrinterFile |
    | e957 | Sensor |
    | e958 | StorageOptical |
    | e95a | Communications |
    | e95b | Headset |
    | e95d | Projector |
    | e95e | Health |
    | e95f | Wire |
    | e960 | Webcam2 |
    | e961 | Input |
    | e962 | Mouse |
    | e963 | Smartcard |
    | e964 | SmartcardVirtual |
    | e965 | MediaStorageTower |
    | e966 | ReturnKeySm |
    | e967 | GameConsole |
    | e968 | Network |
    | e969 | StorageNetworkWireless |
    | e96a | StorageTape |
    | e96d | ChevronUpSmall |
    | e96e | ChevronDownSmall |
    | e96f | ChevronLeftSmall |
    | e970 | ChevronRightSmall |
    | e971 | ChevronUpMed |
    | e972 | ChevronDownMed |
    | e973 | ChevronLeftMed |
    | e974 | ChevronRightMed |
    | e975 | Devices2 |
    | e976 | ExpandTile |
    | e977 | PC1 |
    | e978 | PresenceChicklet |
    | e979 | PresenceChickletVideo |
    | e97a | Reply |
    | e97b | SetTile |
    | e97c | Type |
    | e97d | Korean |
    | e97e | HalfAlpha |
    | e97f | FullAlpha |
    | e980 | Key12On |
    | e981 | ChineseChangjie |
    | e982 | QWERTYOn |
    | e983 | QWERTYOff |
    | e984 | ChineseQuick |
    | e985 | Japanese |
    | e986 | FullHiragana |
    | e987 | FullKatakana |
    | e988 | HalfKatakana |
    | e989 | ChineseBoPoMoFo |
    | e98a | ChinesePinyin |
    | e98f | ConstructionCone |
    | e990 | XboxOneConsole |
    | e992 | Volume0 |
    | e993 | Volume1 |
    | e994 | Volume2 |
    | e995 | Volume3 |
    | e996 | BatteryUnknown |
    | e998 | WifiAttentionOverlay |
    | e99a | Robot |
    | e9a1 | TapAndSend |
    | e9a4 | TextBulletListSquare |
    | e9a6 | FitPage |
    | e9a8 | PasswordKeyShow |
    | e9a9 | PasswordKeyHide |
    | e9aa | BidiLtr |
    | e9ab | BidiRtl |
    | e9ac | ForwardSm |
    | e9ad | CommaKey |
    | e9ae | DashKey |
    | e9af | DullSoundKey |
    | e9b0 | HalfDullSound |
    | e9b1 | RightDoubleQuote |
    | e9b2 | LeftDoubleQuote |
    | e9b3 | PuncKeyRightBottom |
    | e9b4 | PuncKey1 |
    | e9b5 | PuncKey2 |
    | e9b6 | PuncKey3 |
    | e9b7 | PuncKey4 |
    | e9b8 | PuncKey5 |
    | e9b9 | PuncKey6 |
    | e9ba | PuncKey9 |
    | e9bb | PuncKey7 |
    | e9bc | PuncKey8 |
    | e9ca | Frigid |
    | e9ce | Unknown |
    | e9d2 | AreaChart |
    | e9d5 | CheckList |
    | e9d9 | Diagnostic |
    | e9e9 | Equalizer |
    | e9f3 | Process |
    | e9f5 | Processing |
    | e9f9 | ReportDocument |

    ### PUA EA00-EC00

    | ea0c | VideoSolid |
    | ea0d | MixedMediaBadge |
    | ea14 | DisconnectDisplay |
    | ea18 | Shield |
    | ea1f | Info2 |
    | ea21 | ActionCenterAsterisk |
    | ea24 | Beta |
    | ea35 | SaveCopy |
    | ea37 | List |
    | ea38 | Asterisk |
    | ea39 | ErrorBadge |
    | ea3a | CircleRing |
    | ea3b | CircleFill |
    | ea3c | MergeCall |
    | ea3d | PrivateCall |
    | ea3f | Record2 |
    | ea40 | AllAppsMirrored |
    | ea41 | BookmarksMirrored |
    | ea42 | BulletedListMirrored |
    | ea43 | CallForwardInternationalMirrored |
    | ea44 | CallForwardRoamingMirrored |
    | ea47 | ChromeBackMirrored |
    | ea48 | ClearSelectionMirrored |
    | ea49 | ClosePaneMirrored |
    | ea4a | ContactInfoMirrored |
    | ea4b | DockRightMirrored |
    | ea4c | DockLeftMirrored |
    | ea4e | ExpandTileMirrored |
    | ea4f | GoMirrored |
    | ea50 | GripperResizeMirrored |
    | ea51 | HelpMirrored |
    | ea52 | ImportMirrored |
    | ea53 | ImportAllMirrored |
    | ea54 | LeaveChatMirrored |
    | ea55 | ListMirrored |
    | ea56 | MailForwardMirrored |
    | ea57 | MailReplyMirrored |
    | ea58 | MailReplyAllMirrored |
    | ea5b | OpenPaneMirrored |
    | ea5c | OpenWithMirrored |
    | ea5e | ParkingLocationMirrored |
    | ea5f | ResizeMouseMediumMirrored |
    | ea60 | ResizeMouseSmallMirrored |
    | ea61 | ResizeMouseTallMirrored |
    | ea62 | ResizeTouchNarrowerMirrored |
    | ea63 | SendMirrored |
    | ea64 | SendFillMirrored |
    | ea65 | ShowResultsMirrored |
    | ea69 | Media |
    | ea6a | SyncError |
    | ea6c | Devices3 |
    | ea79 | SlowMotionOn |
    | ea80 | Lightbulb |
    | ea81 | StatusCircle |
    | ea82 | StatusTriangle |
    | ea83 | StatusError |
    | ea84 | StatusWarning |
    | ea85 | VolumeDisabled |
    | ea86 | Puzzle |
    | ea89 | CalendarSolid |
    | ea8a | HomeSolid |
    | ea8b | ParkingLocationSolid |
    | ea8c | ContactSolid |
    | ea8d | ConstructionSolid |
    | ea8e | AccidentSolid |
    | ea8f | Ringer |
    | ea90 | PDF |
    | ea91 | ThoughtBubble |
    | ea92 | HeartBroken |
    | ea93 | BatteryCharging10 |
    | ea94 | BatterySaver9 |
    | ea95 | BatterySaver10 |
    | ea97 | CallForwardingMirrored |
    | ea98 | MultiSelectMirrored |
    | ea99 | Broom |
    | eac2 | ForwardCall |
    | eac7 | DesktopLeafTwo |
    | ead4 | Emojiplay |
    | ead5 | EmojiBrush |
    | ead6 | EyeTracking |
    | ead7 | EyeTrackingText |
    | eadf | Trackers |
    | eafc | Market |
    | eb05 | PieSingle |
    | eb0f | StockUp |
    | eb11 | StockDown |
    | eb19 | ClicktoDoOff |
    | eb1d | ClicktoDo |
    | eb3b | GenericApp |
    | eb3c | Design |
    | eb41 | Website |
    | eb42 | Drop |
    | eb44 | Radar |
    | eb47 | BusSolid |
    | eb48 | FerrySolid |
    | eb49 | StartPointSolid |
    | eb4a | StopPointSolid |
    | eb4b | EndPointSolid |
    | eb4c | AirplaneSolid |
    | eb4d | TrainSolid |
    | eb4e | WorkSolid |
    | eb4f | ReminderFill |
    | eb50 | Reminder |
    | eb51 | Heart |
    | eb52 | HeartFill |
    | eb55 | EthernetError |
    | eb56 | EthernetWarning |
    | eb57 | StatusConnecting1 |
    | eb58 | StatusConnecting2 |
    | eb59 | StatusUnsecure |
    | eb5a | WifiError0 |
    | eb5b | WifiError1 |
    | eb5c | WifiError2 |
    | eb5d | WifiError3 |
    | eb5e | WifiError4 |
    | eb5f | WifiWarning0 |
    | eb60 | WifiWarning1 |
    | eb61 | WifiWarning2 |
    | eb62 | WifiWarning3 |
    | eb63 | WifiWarning4 |
    | eb66 | Devices4 |
    | eb67 | NUIIris |
    | eb68 | NUIFace |
    | eb77 | GatewayRouter |
    | eb7e | EditMirrored |
    | eb82 | NUIFPStartSlideHand |
    | eb83 | NUIFPStartSlideAction |
    | eb84 | NUIFPContinueSlideHand |
    | eb85 | NUIFPContinueSlideAction |
    | eb86 | NUIFPRollRightHand |
    | eb87 | NUIFPRollRightHandAction |
    | eb88 | NUIFPRollLeftHand |
    | eb89 | NUIFPRollLeftAction |
    | eb8a | NUIFPPressHand |
    | eb8b | NUIFPPressAction |
    | eb8c | NUIFPPressRepeatHand |
    | eb8d | NUIFPPressRepeatAction |
    | eb90 | StatusErrorFull |
    | eb91 | TaskViewExpanded |
    | eb95 | Certificate |
    | eb96 | BackSpaceQWERTYLg |
    | eb97 | ReturnKeyLg |
    | eb9d | FastForward |
    | eb9e | Rewind |
    | eb9f | Photo2 |
    | eba0 | MobBattery0 |
    | eba1 | MobBattery1 |
    | eba2 | MobBattery2 |
    | eba3 | MobBattery3 |
    | eba4 | MobBattery4 |
    | eba5 | MobBattery5 |
    | eba6 | MobBattery6 |
    | eba7 | MobBattery7 |
    | eba8 | MobBattery8 |
    | eba9 | MobBattery9 |
    | ebaa | MobBattery10 |
    | ebab | MobBatteryCharging0 |
    | ebac | MobBatteryCharging1 |
    | ebad | MobBatteryCharging2 |
    | ebae | MobBatteryCharging3 |
    | ebaf | MobBatteryCharging4 |
    | ebb0 | MobBatteryCharging5 |
    | ebb1 | MobBatteryCharging6 |
    | ebb2 | MobBatteryCharging7 |
    | ebb3 | MobBatteryCharging8 |
    | ebb4 | MobBatteryCharging9 |
    | ebb5 | MobBatteryCharging10 |
    | ebb6 | MobBatterySaver0 |
    | ebb7 | MobBatterySaver1 |
    | ebb8 | MobBatterySaver2 |
    | ebb9 | MobBatterySaver3 |
    | ebba | MobBatterySaver4 |
    | ebbb | MobBatterySaver5 |
    | ebbc | MobBatterySaver6 |
    | ebbd | MobBatterySaver7 |
    | ebbe | MobBatterySaver8 |
    | ebbf | MobBatterySaver9 |
    | ebc0 | MobBatterySaver10 |
    | ebc3 | DictionaryCloud |
    | ebc4 | ResetDrive |
    | ebc5 | VolumeBars |
    | ebc6 | Project |
    | ebd2 | AdjustHologram |
    | ebd3 | CloudDownload |
    | ebd4 | MobWifiCallBars |
    | ebd5 | MobWifiCall0 |
    | ebd6 | MobWifiCall1 |
    | ebd7 | MobWifiCall2 |
    | ebd8 | MobWifiCall3 |
    | ebd9 | MobWifiCall4 |
    | ebda | Family |
    | ebdb | LockFeedback |
    | ebde | DeviceDiscovery |
    | ebe6 | WindDirection |
    | ebe7 | RightArrowKeyTime0 |
    | ebe8 | Bug |
    | ebfc | TabletMode |
    | ebfd | StatusCircleLeft |
    | ebfe | StatusTriangleLeft |
    | ebff | StatusErrorLeft |
    | ec00 | StatusWarningLeft |
    | ec02 | MobBatteryUnknown |
    | ec05 | NetworkTower |
    | ec06 | CityNext |
    | ec07 | CityNext2 |
    | ec08 | Courthouse |
    | ec09 | Groceries |
    | ec0a | Sustainable |
    | ec0b | BuildingEnergy |
    | ec11 | ToggleFilled |
    | ec12 | ToggleBorder |
    | ec13 | SliderThumb |
    | ec14 | ToggleThumb |
    | ec15 | MiracastLogoSmall |
    | ec16 | MiracastLogoLarge |
    | ec19 | PLAP |
    | ec1b | Badge |
    | ec1e | SignalRoaming |
    | ec20 | MobileLocked |
    | ec24 | InsiderHubApp |
    | ec25 | PersonalFolder |
    | ec26 | HomeGroup |
    | ec27 | MyNetwork |
    | ec31 | KeyboardFull |
    | ec32 | Cafe |
    | ec34 | FormatText |
    | ec37 | MobSignal1 |
    | ec38 | MobSignal2 |
    | ec39 | MobSignal3 |
    | ec3a | MobSignal4 |
    | ec3b | MobSignal5 |
    | ec3c | MobWifi1 |
    | ec3d | MobWifi2 |
    | ec3e | MobWifi3 |
    | ec3f | MobWifi4 |
    | ec40 | MobAirplane |
    | ec41 | MobBluetooth |
    | ec42 | MobActionCenter |
    | ec43 | MobLocation |
    | ec44 | MobWifiHotspot |
    | ec45 | LanguageJpn |
    | ec46 | MobQuietHours |
    | ec47 | MobDrivingMode |
    | ec48 | SpeedOff |
    | ec49 | SpeedMedium |
    | ec4a | SpeedHigh |
    | ec4e | ThisPC |
    | ec4f | MusicNote |
    | ec50 | FileExplorer |
    | ec51 | FileExplorerApp |
    | ec52 | LeftArrowKeyTime0 |
    | ec54 | MicOff |
    | ec55 | MicSleep |
    | ec56 | MicError |
    | ec57 | PlaybackRate1x |
    | ec58 | PlaybackRateOther |
    | ec59 | CashDrawer |
    | ec5a | BarcodeScanner |
    | ec5b | ReceiptPrinter |
    | ec5c | MagStripeReader |
    | ec61 | CompletedSolid |
    | ec64 | CompanionApp |
    | ec6c | Favicon2 |
    | ec6d | SwipeRevealArt |
    | ec71 | MicOn |
    | ec72 | MicClipping |
    | ec74 | TabletSelected |
    | ec75 | MobileSelected |
    | ec76 | LaptopSelected |
    | ec77 | TVMonitorSelected |
    | ec7a | DeveloperTools |
    | ec7e | MobCallForwarding |
    | ec7f | MobCallForwardingMirrored |
    | ec80 | BodyCam |
    | ec81 | PoliceCar |
    | ec83 | UpdateStatusDot2 |
    | ec87 | Draw |
    | ec88 | DrawSolid |
    | ec8a | LowerBrightness |
    | ec8f | ScrollUpDown |
    | ec91 | Uninstall |
    | ec92 | DateTime |
    | ec94 | HoloLens |
    | ec9c | CloudNotSynced |
    | eca5 | Tiles |
    | eca7 | PartyLeader |
    | ecaa | AppIconDefault |
    | ecad | Calories |
    | ecaf | POI |
    | ecb9 | BandBattery0 |
    | ecba | BandBattery1 |
    | ecbb | BandBattery2 |
    | ecbc | BandBattery3 |
    | ecbd | BandBattery4 |
    | ecbe | BandBattery5 |
    | ecbf | BandBattery6 |
    | ecc4 | AddSurfaceHub |
    | ecc5 | DevUpdate |
    | ecc6 | Unit |
    | ecc8 | AddTo |
    | ecc9 | RemoveFrom |
    | ecca | RadioBtnOff |
    | eccb | RadioBtnOn |
    | eccc | RadioBullet2 |
    | eccd | ExploreContent |
    | ece4 | Blocked2 |
    | ece7 | ScrollMode |
    | ece8 | ZoomMode |
    | ece9 | PanMode |
    | ecf0 | WiredUSB |
    | ecf1 | WirelessUSB |
    | ecf3 | USBSafeConnect |

    ### PUA ED00-EF00

    | ed0c | ActionCenterNotificationMirrored |
    | ed0d | ActionCenterMirrored |
    | ed0e | SubscriptionAdd |
    | ed10 | ResetDevice |
    | ed11 | SubscriptionAddMirrored |
    | ed14 | QRCode |
    | ed15 | Feedback |
    | ed1a | Hide |
    | ed1e | Subtitles |
    | ed1f | SubtitlesAudio |
    | ed21 | RestartUpdate2 |
    | ed25 | OpenFolderHorizontal |
    | ed28 | CalendarMirrored |
    | ed2a | MobeSIM |
    | ed2b | MobeSIMNoProfile |
    | ed2c | MobeSIMLocked |
    | ed2d | MobeSIMBusy |
    | ed2e | SignalError |
    | ed2f | StreamingEnterprise |
    | ed30 | Headphone0 |
    | ed31 | Headphone1 |
    | ed32 | Headphone2 |
    | ed33 | Headphone3 |
    | ed35 | Apps |
    | ed39 | KeyboardBrightness |
    | ed3a | KeyboardLowerBrightness |
    | ed3c | SkipBack10 |
    | ed3d | SkipForward30 |
    | ed41 | TreeFolderFolder |
    | ed42 | TreeFolderFolderFill |
    | ed43 | TreeFolderFolderOpen |
    | ed44 | TreeFolderFolderOpenFill |
    | ed47 | MultimediaDMP |
    | ed4c | KeyboardOneHanded |
    | ed4d | Narrator |
    | ed53 | EmojiTabPeople |
    | ed54 | EmojiTabSmilesAnimals |
    | ed55 | EmojiTabCelebrationObjects |
    | ed56 | EmojiTabFoodPlants |
    | ed57 | EmojiTabTransitPlaces |
    | ed58 | EmojiTabSymbols |
    | ed59 | EmojiTabTextSmiles |
    | ed5a | EmojiTabFavorites |
    | ed5b | EmojiSwatch |
    | ed5c | ConnectApp |
    | ed5d | CompanionDeviceFramework |
    | ed5e | Ruler |
    | ed5f | FingerInking |
    | ed60 | StrokeErase |
    | ed61 | PointErase |
    | ed62 | ClearAllInk |
    | ed63 | Pencil |
    | ed64 | Marker |
    | ed65 | InkingCaret |
    | ed66 | InkingColorOutline |
    | ed67 | InkingColorFill |
    | eda2 | HardDrive |
    | eda3 | NetworkAdapter |
    | eda4 | Touchscreen |
    | eda5 | NetworkPrinter |
    | eda6 | CloudPrinter |
    | eda7 | KeyboardShortcut |
    | eda8 | BrushSize |
    | eda9 | NarratorForward |
    | edaa | NarratorForwardMirrored |
    | edab | SyncBadge12 |
    | edac | RingerBadge12 |
    | edad | AsteriskBadge12 |
    | edae | ErrorBadge12 |
    | edaf | CircleRingBadge12 |
    | edb0 | CircleFillBadge12 |
    | edb1 | ImportantBadge12 |
    | edb3 | MailBadge12 |
    | edb4 | PauseBadge12 |
    | edb5 | PlayBadge12 |
    | edc6 | PenWorkspace |
    | edd5 | CaretLeft8 |
    | edd6 | CaretRight8 |
    | edd7 | CaretUp8 |
    | edd8 | CaretDown8 |
    | edd9 | CaretLeftSolid8 |
    | edda | CaretRightSolid8 |
    | eddb | CaretUpSolid8 |
    | eddc | CaretDownSolid8 |
    | ede0 | Strikethrough |
    | ede1 | Export |
    | ede2 | ExportMirrored |
    | ede3 | ButtonMenu |
    | ede4 | CloudSearch |
    | ede5 | PinyinIMELogo |
    | edfb | CalligraphyPen |
    | ee35 | ReplyMirrored |
    | ee3f | LockscreenDesktop |
    | ee40 | TaskViewSettings |
    | ee41 | FullHiraganaPrivateMode |
    | ee42 | FullKatakanaPrivateMode |
    | ee43 | HalfAlphaPrivateMode |
    | ee44 | HalfKatakanaPrivateMode |
    | ee45 | FullAlphaPrivateMode |
    | ee47 | MiniExpand2Mirrored |
    | ee49 | MiniContract2Mirrored |
    | ee4a | Play36 |
    | ee56 | PenPalette |
    | ee57 | GuestUser |
    | ee63 | SettingsBattery |
    | ee64 | TaskbarPhone |
    | ee65 | LockScreenGlance |
    | ee6f | GenericScan |
    | ee71 | ImageExport |
    | ee77 | WifiEthernet |
    | ee79 | ActionCenterQuiet |
    | ee7a | ActionCenterQuietNotification |
    | ee7e | FIDOPasskey |
    | ee92 | TrackersMirrored |
    | ee93 | DateTimeMirrored |
    | ee94 | Wheel |
    | ee95 | StopSolid |
    | eea0 | RAM |
    | eea1 | CPU |
    | eea3 | VirtualMachineGroup |
    | eeca | ButtonView2 |
    | ef15 | PenWorkspaceMirrored |
    | ef16 | PenPaletteMirrored |
    | ef17 | StrokeEraseMirrored |
    | ef18 | PointEraseMirrored |
    | ef19 | ClearAllInkMirrored |
    | ef1f | BackgroundToggle |
    | ef20 | Marquee |
    | ef2c | ChromeCloseContrast |
    | ef2d | ChromeMinimizeContrast |
    | ef2e | ChromeMaximizeContrast |
    | ef2f | ChromeRestoreContrast |
    | ef31 | TrafficLight |
    | ef3b | Replay |
    | ef3c | Eyedropper |
    | ef3d | LineDisplay |
    | ef3e | PINPad |
    | ef3f | SignatureCapture |
    | ef40 | ChipCardCreditCardReader |
    | ef42 | MarketDown |
    | ef58 | PlayerSettings |
    | ef60 | TextEdit |
    | ef6b | LandscapeOrientation |
    | ef90 | Flow |
    | efa5 | Touchpad |
    | efa9 | Speech |
    | efda | AppIconDefaultAdd |
    | efff | CRMScheduleReports |

    ### PUA F000-F200

    | f000 | KnowledgeArticle |
    | f003 | Relationship |
    | f012 | ZipFolder |
    | f080 | DefaultAPN |
    | f081 | UserAPN |
    | f085 | DoublePinyin |
    | f08c | BlueLight |
    | f08d | CaretSolidLeft |
    | f08e | CaretSolidDown |
    | f08f | CaretSolidRight |
    | f090 | CaretSolidUp |
    | f093 | ButtonA |
    | f094 | ButtonB |
    | f095 | ButtonY |
    | f096 | ButtonX |
    | f0ad | ArrowUp8 |
    | f0ae | ArrowDown8 |
    | f0af | ArrowRight8 |
    | f0b0 | ArrowLeft8 |
    | f0b2 | QuarentinedItems |
    | f0b3 | QuarentinedItemsMirrored |
    | f0b4 | Protractor |
    | f0b5 | ChecklistMirrored |
    | f0b6 | StatusCircle7 |
    | f0b7 | StatusCheckmark7 |
    | f0b8 | StatusErrorCircle7 |
    | f0b9 | Connected |
    | f0c6 | PencilFill |
    | f0c7 | CalligraphyFill |
    | f0ca | QuarterStarLeft |
    | f0cb | QuarterStarRight |
    | f0cc | ThreeQuarterStarLeft |
    | f0cd | ThreeQuarterStarRight |
    | f0ce | QuietHoursBadge12 |
    | f0d2 | BackMirrored |
    | f0d3 | ForwardMirrored |
    | f0d5 | ChromeBackContrast |
    | f0d6 | ChromeBackContrastMirrored |
    | f0d7 | ChromeBackToWindowContrast |
    | f0d8 | ChromeFullScreenContrast |
    | f0e2 | GridView |
    | f0e3 | ClipboardList |
    | f0e4 | ClipboardListMirrored |
    | f0e5 | OutlineQuarterStarLeft |
    | f0e6 | OutlineQuarterStarRight |
    | f0e7 | OutlineHalfStarLeft |
    | f0e8 | OutlineHalfStarRight |
    | f0e9 | OutlineThreeQuarterStarLeft |
    | f0ea | OutlineThreeQuarterStarRight |
    | f0eb | SpatialVolume0 |
    | f0ec | SpatialVolume1 |
    | f0ed | SpatialVolume2 |
    | f0ee | SpatialVolume3 |
    | f0ef | ApplicationGuard |
    | f0f7 | OutlineStarLeftHalf |
    | f0f8 | OutlineStarRightHalf |
    | f0f9 | ChromeAnnotateContrast |
    | f0fb | DefenderBadge12 |
    | f103 | DetachablePC |
    | f108 | LeftStick |
    | f109 | RightStick |
    | f10a | TriggerLeft |
    | f10b | TriggerRight |
    | f10c | BumperLeft |
    | f10d | BumperRight |
    | f10e | Dpad |
    | f110 | EnglishPunctuation |
    | f111 | ChinesePunctuation |
    | f119 | HMD |
    | f11b | CtrlSpatialRight |
    | f126 | PaginationDotOutline10 |
    | f127 | PaginationDotSolid10 |
    | f128 | StrokeErase2 |
    | f129 | SmallErase |
    | f12a | LargeErase |
    | f12b | FolderHorizontal |
    | f12e | MicrophoneListening |
    | f12f | StatusExclamationCircle7 |
    | f131 | Video360 |
    | f133 | GiftboxOpen |
    | f136 | StatusCircleOuter |
    | f137 | StatusCircleInner |
    | f138 | StatusCircleRing |
    | f139 | StatusTriangleOuter |
    | f13a | StatusTriangleInner |
    | f13b | StatusTriangleExclamation |
    | f13c | StatusCircleExclamation |
    | f13d | StatusCircleErrorX |
    | f13e | StatusCircleCheckmark |
    | f13f | StatusCircleInfo |
    | f140 | StatusCircleBlock |
    | f141 | StatusCircleBlock2 |
    | f142 | StatusCircleQuestionMark |
    | f143 | StatusCircleSync |
    | f146 | Dial1 |
    | f147 | Dial2 |
    | f148 | Dial3 |
    | f149 | Dial4 |
    | f14a | Dial5 |
    | f14b | Dial6 |
    | f14c | Dial7 |
    | f14d | Dial8 |
    | f14e | Dial9 |
    | f14f | Dial10 |
    | f150 | Dial11 |
    | f151 | Dial12 |
    | f152 | Dial13 |
    | f153 | Dial14 |
    | f154 | Dial15 |
    | f155 | Dial16 |
    | f156 | DialShape1 |
    | f157 | DialShape2 |
    | f158 | DialShape3 |
    | f159 | DialShape4 |
    | f15f | ClosedCaptionsInternational |
    | f161 | TollSolid |
    | f163 | TrafficCongestionSolid |
    | f164 | ExploreContentSingle |
    | f165 | CollapseContent |
    | f166 | CollapseContentSingle |
    | f167 | InfoSolid |
    | f168 | GroupList |
    | f169 | CaretBottomRightSolidCenter8 |
    | f16a | ProgressRingDots |
    | f16b | Checkbox14 |
    | f16c | CheckboxComposite14 |
    | f16d | CheckboxIndeterminateCombo14 |
    | f16e | CheckboxIndeterminateCombo |
    | f175 | StatusPause7 |
    | f17f | CharacterAppearance |
    | f180 | Lexicon |
    | f182 | ScreenTime |
    | f191 | HeadlessDevice |
    | f193 | NetworkSharing |
    | f19d | EyeGaze |
    | f19e | ToggleLeft |
    | f19f | ToggleRight |
    | f1ad | WindowsInsider |
    | f1cb | ChromeSwitch |
    | f1cc | ChromeSwitchContast |
    | f1d8 | StatusCheckmark |
    | f1d9 | StatusCheckmarkLeft |
    | f20c | KeyboardLeftAligned |
    | f20d | KeyboardRightAligned |
    | f210 | KeyboardSettings |
    | f211 | NetworkPhysical |
    | f22c | IOT |
    | f22e | UnknownMirrored |
    | f246 | ViewDashboard |
    | f259 | ExploitProtectionSettings |
    | f260 | KeyboardNarrow |
    | f261 | Keyboard12Key |
    | f26b | KeyboardDock |
    | f26c | KeyboardUndock |
    | f26d | KeyboardLeftDock |
    | f26e | KeyboardRightDock |
    | f270 | Ear |
    | f271 | PointerHand |
    | f272 | Bullseye |
    | f28b | DocumentApproval |
    | f2b7 | LocaleLanguage |

    ### PUA F300-F500

    | f112 | ReadOutLoud |
    | f117 | ProjectToDevice |
    | f120 | TaskManagerApp |
    | f191 | HeadlessDevice |
    | f196 | Beaker |
    | f1ad | WindowsInsider |
    | f1b1 | PowerButtonUpdate2 |
    | f1e8 | LeafTwo |
    | f232 | GridViewSmall |
    | f27c | Earbudsingle |
    | f27f | HearingAid |
    | f285 | MobSnooze |
    | f2a3 | MobNotificationBell |
    | f2a5 | MobNotificationBellFilled |
    | f2a8 | MobSnoozeFilled |
    | f2c7 | BulletedList2 |
    | f2c8 | BulletedList2Mirrored |
    | f2d9 | CirclePause |
    | f32a | PassiveAuthentication |
    | f354 | ColorSolid |
    | f384 | NetworkOffline |
    | f385 | NetworkConnected |
    | f386 | NetworkConnectedCheckmark |
    | f3b1 | SignOut |
    | f3cc | StatusInfo |
    | f3cd | StatusInfoLeft |
    | f3e2 | NearbySharing |
    | f3e7 | CtrlSpatialLeft |
    | f404 | InteractiveDashboard |
    | f405 | DeclineCall |
    | f406 | ClippingTool |
    | f407 | RectangularClipping |
    | f408 | FreeFormClipping |
    | f413 | CopyTo |
    | f427 | IDBadge |
    | f432 | BatterySaver |
    | f439 | DynamicLock |
    | f45e | PenTips |
    | f45f | PenTipsMirrored |
    | f460 | HWPJoin |
    | f461 | HWPInsert |
    | f462 | HWPStrikeThrough |
    | f463 | HWPScratchOut |
    | f464 | HWPSplit |
    | f465 | HWPNewLine |
    | f466 | HWPOverwrite |
    | f473 | MobWifiWarning1 |
    | f474 | MobWifiWarning2 |
    | f475 | MobWifiWarning3 |
    | f476 | MobWifiWarning4 |
    | f47f | MicLocationCombo |
    | f49a | Globe2 |
    | f4a5 | SpecialEffectSize |
    | f4a9 | GIF |
    | f4aa | Sticker2 |
    | f4bd | Snooze |
    | f4be | SurfaceHubSelected |
    | f4bf | HoloLensSelected |
    | f4c0 | Earbud |
    | f4c3 | MixVolumes |
    | f540 | Safe |
    | f552 | LaptopSecure |
    | f56d | PrintDefault |
    | f56e | PageMirrored |
    | f56f | LandscapeOrientationMirrored |
    | f570 | ColorOff |
    | f571 | PrintAllPages |
    | f572 | PrintCustomRange |
    | f573 | PageMarginPortraitNarrow |
    | f574 | PageMarginPortraitNormal |
    | f575 | PageMarginPortraitModerate |
    | f576 | PageMarginPortraitWide |
    | f577 | PageMarginLandscapeNarrow |
    | f578 | PageMarginLandscapeNormal |
    | f579 | PageMarginLandscapeModerate |
    | f57a | PageMarginLandscapeWide |
    | f57b | CollateLandscape |
    | f57c | CollatePortrait |
    | f57d | CollatePortraitSeparated |
    | f57e | DuplexLandscapeOneSided |
    | f57f | DuplexLandscapeOneSidedMirrored |
    | f580 | DuplexLandscapeTwoSidedLongEdge |
    | f581 | DuplexLandscapeTwoSidedLongEdgeMirrored |
    | f582 | DuplexLandscapeTwoSidedShortEdge |
    | f583 | DuplexLandscapeTwoSidedShortEdgeMirrored |
    | f584 | DuplexPortraitOneSided |
    | f585 | DuplexPortraitOneSidedMirrored |
    | f586 | DuplexPortraitTwoSidedLongEdge |
    | f587 | DuplexPortraitTwoSidedLongEdgeMirrored |
    | f588 | DuplexPortraitTwoSidedShortEdge |
    | f589 | DuplexPortraitTwoSidedShortEdgeMirrored |
    | f58a | PPSOneLandscape |
    | f58b | PPSTwoLandscape |
    | f58c | PPSTwoPortrait |
    | f58d | PPSFourLandscape |
    | f58e | PPSFourPortrait |
    | f58f | HolePunchOff |
    | f590 | HolePunchPortraitLeft |
    | f591 | HolePunchPortraitRight |
    | f592 | HolePunchPortraitTop |
    | f593 | HolePunchPortraitBottom |
    | f594 | HolePunchLandscapeLeft |
    | f595 | HolePunchLandscapeRight |
    | f596 | HolePunchLandscapeTop |
    | f597 | HolePunchLandscapeBottom |
    | f598 | StaplingOff |
    | f599 | StaplingPortraitTopLeft |
    | f59a | StaplingPortraitTopRight |
    | f59b | StaplingPortraitBottomRight |
    | f59c | StaplingPortraitTwoLeft |
    | f59d | StaplingPortraitTwoRight |
    | f59e | StaplingPortraitTwoTop |
    | f59f | StaplingPortraitTwoBottom |
    | f5a0 | StaplingPortraitBookBinding |
    | f5a1 | StaplingLandscapeTopLeft |
    | f5a2 | StaplingLandscapeTopRight |
    | f5a3 | StaplingLandscapeBottomLeft |
    | f5a4 | StaplingLandscapeBottomRight |
    | f5a5 | StaplingLandscapeTwoLeft |
    | f5a6 | StaplingLandscapeTwoRight |
    | f5a7 | StaplingLandscapeTwoTop |
    | f5a8 | StaplingLandscapeTwoBottom |
    | f5a9 | StaplingLandscapeBookBinding |
    | f5aa | StatusDataTransferRoaming |
    | f5ab | MobSIMError |
    | f5ac | CollateLandscapeSeparated |
    | f5ad | PPSOnePortrait |
    | f5ae | StaplingPortraitBottomLeft |
    | f5b0 | PlaySolid |
    | f5e7 | RepeatOff |
    | f5ed | Set |
    | f5ee | SetSolid |
    | f5ef | FuzzyReading |
    | f5f2 | VerticalBattery0 |
    | f5f3 | VerticalBattery1 |
    | f5f4 | VerticalBattery2 |
    | f5f5 | VerticalBattery3 |
    | f5f6 | VerticalBattery4 |
    | f5f7 | VerticalBattery5 |
    | f5f8 | VerticalBattery6 |
    | f5f9 | VerticalBattery7 |
    | f5fa | VerticalBattery8 |
    | f5fb | VerticalBattery9 |
    | f5fc | VerticalBattery10 |
    | f5fd | VerticalBatteryCharging0 |
    | f5fe | VerticalBatteryCharging1 |
    | f5ff | VerticalBatteryCharging2 |

    ### PUA F600-F800

    | f600 | VerticalBatteryCharging3 |
    | f601 | VerticalBatteryCharging4 |
    | f602 | VerticalBatteryCharging5 |
    | f603 | VerticalBatteryCharging6 |
    | f604 | VerticalBatteryCharging7 |
    | f605 | VerticalBatteryCharging8 |
    | f606 | VerticalBatteryCharging9 |
    | f607 | VerticalBatteryCharging10 |
    | f608 | VerticalBatteryUnknown |
    | f618 | SIMError |
    | f619 | SIMMissing |
    | f61a | SIMLock |
    | f61b | eSIM |
    | f61c | eSIMNoProfile |
    | f61d | eSIMLocked |
    | f61e | eSIMBusy |
    | f61f | NoiseCancelation |
    | f620 | NoiseCancelationOff |
    | f623 | MusicSharing |
    | f624 | MusicSharingOff |
    | f63c | CircleShapeSolid |
    | f657 | WifiCallBars |
    | f658 | WifiCall0 |
    | f659 | WifiCall1 |
    | f65a | WifiCall2 |
    | f65b | WifiCall3 |
    | f65c | WifiCall4 |
    | f67b | Pen |
    | f683 | TextSelect |
    | f684 | TextNavigate |
    | f698 | PinyinIMELogo2 |
    | f69b | UserRemove |
    | f69e | CHTLanguageBar |
    | f6a9 | ComposeMode |
    | f6b8 | ExpressiveInputEntry |
    | f6ba | EmojiTabMoreSymbols |
    | f6c4 | PhoneScreen |
    | f6c5 | AlertUrgent |
    | f6c6 | PhoneDesktop |
    | f6fa | WebSearch |
    | f712 | Kiosk |
    | f714 | RTTLogo |
    | f715 | VoiceCall |
    | f716 | GoToMessage |
    | f71a | ReturnToCall |
    | f71c | StartPresenting |
    | f71d | StopPresenting |
    | f71e | ProductivityMode |
    | f738 | SetHistoryStatus |
    | f739 | SetHistoryStatus2 |
    | f73d | Keyboardsettings20 |
    | f73e | OneHandedRight20 |
    | f73f | OneHandedLeft20 |
    | f740 | Split20 |
    | f741 | Full20 |
    | f742 | Handwriting20 |
    | f743 | ChevronLeft20 |
    | f744 | ChevronLeft32 |
    | f745 | ChevronRight20 |
    | f746 | ChevronRight32 |
    | f763 | Event12 |
    | f781 | MicOff2 |
    | f785 | DeliveryOptimization |
    | f78a | CancelMedium |
    | f78b | SearchMedium |
    | f78c | AcceptMedium |
    | f78d | RevealPasswordMedium |
    | f7ad | DeleteWord |
    | f7ae | DeleteWordFill |
    | f7af | DeleteLines |
    | f7b0 | DeleteLinesFill |
    | f7b1 | InstertWords |
    | f7b2 | InstertWordsFill |
    | f7b3 | JoinWords |
    | f7b4 | JoinWordsFill |
    | f7b5 | OverwriteWords |
    | f7b6 | OverwriteWordsFill |
    | f7b7 | AddNewLine |
    | f7b8 | AddNewLineFill |
    | f7b9 | OverwriteWordsKorean |
    | f7ba | OverwriteWordsFillKorean |
    | f7bb | EducationIcon |
    | f7ed | WindowSnipping |
    | f7ee | VideoCapture |
    | f809 | StatusSecured |
    | f83b | NarratorApp |
    | f83d | PowerButtonUpdate |
    | f83e | RestartUpdate |
    | f83f | UpdateStatusDot |
    | f847 | Eject |
    | f87b | Spelling |
    | f87c | SpellingKorean |
    | f87d | SpellingSerbian |
    | f87e | SpellingChinese |
    | f89a | FolderSelect |
    | f8a5 | SmartScreen |
    | f8a6 | ExploitProtection |
    | f8aa | AddBold |
    | f8ab | SubtractBold |
    | f8ac | BackSolidBold |
    | f8ad | ForwardSolidBold |
    | f8ae | PauseBold |
    | f8af | ClickSolid |
    | f8b0 | SettingsSolid |
    | f8b1 | MicrophoneSolidBold |
    | f8b2 | SpeechSolidBold |
    | f8b3 | ClickedOutLoudSolidBold |
    | f8c0 | VPNOverlay |
    | f8c1 | VPNRoamingOverly |
    | f8c2 | WifiVPN3 |
    | f8c3 | WifiVPN4 |
    | f8c4 | WifiVPN5 |
    | f8c5 | SignalBarsVPN2 |
    | f8c6 | SignalBarsVPN3 |
    | f8c7 | SignalBarsVPN4 |
    | f8c8 | SignalBarsVPN5 |
    | f8c9 | SignalBarsVPNRoaming3 |
    | f8ca | SignalBarsVPNRoaming4 |
    | f8cb | SignalBarsVPNRoaming5 |
    | f8cc | EthernetVPN |
    """

    def __init__(self, font_path: str):
        """
        Initialize the generator.

        Args:
            font_path: Path to the .ttf font file
        """
        self.font_path = Path(font_path)
        self.font = None
        self.official_icons: Dict[str, str] = {}  # unicode -> name mapping
        self.font_cmap: Dict[int, str] = {}  # Unicode -> glyph name mapping from cmap

    def load_font(self) -> None:
        """Load the font file."""
        if not self.font_path.exists():
            raise FileNotFoundError(f"Font file not found: {self.font_path}")

        print(f"Loading font: {self.font_path}")
        self.font = TTFont(str(self.font_path))
        print(f"Font loaded successfully: {self.font['name'].getBestFullName()}")

    def parse_documentation(self) -> None:
        """Parse the Microsoft documentation to extract icon mappings."""
        print("Parsing Microsoft official documentation...")

        # Extract table rows from documentation
        # Pattern: | e700 | GlobalNavButton |
        pattern = r'\|\s*([a-f0-9]+)\s*\|\s*([A-Za-z0-9_]+)\s*\|'

        matches = re.findall(pattern, self.DOCUMENTATION_CONTENT, re.IGNORECASE)

        for unicode_hex, name in matches:
            unicode_upper = unicode_hex.upper()
            self.official_icons[unicode_upper] = name

        print(f"Extracted {len(self.official_icons)} icons from documentation")

    def get_font_glyphs(self) -> None:
        """Get all available glyphs from the font file using cmap."""
        if not self.font:
            self.load_font()

        print("Extracting glyphs from font file...")

        # Get the character map (cmap) which maps Unicode to glyph names
        self.font_cmap = self.font['cmap'].getBestCmap()

        print(f"Found {len(self.font_cmap)} glyphs in font file")

    def categorize_icon(self, unicode_hex: str, name: str) -> str:
        """
        Categorize an icon based on Unicode range and name.

        Args:
            unicode_hex: Unicode code point
            name: Icon name

        Returns:
            Category key
        """
        try:
            code = int(unicode_hex, 16)
        except ValueError:
            return "uncategorized"

        # Categorize by Unicode range (from Microsoft documentation structure)
        if 0xE700 <= code < 0xE800:
            # E700-E7FF: Basic navigation and actions
            return "navigation"
        elif 0xE800 <= code < 0xE900:
            # E800-E8FF: Media and communication
            if any(keyword in name.lower() for keyword in ['battery', 'signal', 'wifi', 'status', 'vpn']):
                return "status"
            return "media"
        elif 0xE900 <= code < 0xEA00:
            # E900-E9FF: Files and UI elements
            if any(keyword in name.lower() for keyword in ['bar', 'status', 'volume', 'battery']):
                return "status"
            return "files"
        elif 0xEA00 <= code < 0xEB00:
            # EA00-EBFF: Advanced UI and media
            if 'solid' in name.lower():
                return "media"
            return "ui"
        elif 0xEB00 <= code < 0xEC00:
            # EB00-ECFF: Network and connectivity
            return "communication"
        elif 0xEC00 <= code < 0xED00:
            # EC00-EDFF: Devices and settings
            return "devices"
        elif 0xED00 <= code < 0xEF00:
            # ED00-EFFF: Accessibility and input
            return "accessibility"
        elif 0xEF00 <= code < 0xF000:
            # EF00-EFFF: Specialized
            return "specialized"
        elif 0xF000 <= code < 0xF300:
            # F000-F2FF: Business and productivity
            return "business"
        elif 0xF300 <= code < 0xF600:
            # F300-F5FF: Advanced features
            return "advanced"
        elif 0xF600 <= code < 0xF900:
            # F600-F8FF: New and experimental
            return "new"
        else:
            return "uncategorized"

    def generate_keywords(self, name: str, unicode_hex: str, category: str) -> List[str]:
        """
        Generate search keywords for an icon.

        Args:
            name: Icon name
            unicode_hex: Unicode code point
            category: Icon category

        Returns:
            List of keywords
        """
        keywords = set()

        # Add Unicode variations
        keywords.add(unicode_hex.lower())
        keywords.add(f"u{unicode_hex.lower()}")
        keywords.add(f"0x{unicode_hex.lower()}")

        # Add name variations
        keywords.add(name.lower())

        # Split PascalCase/camelCase into words
        words = re.findall(r'[A-Z]?[a-z]+|[A-Z]+(?=[A-Z]|$)', name)
        keywords.update([w.lower() for w in words])

        # Add common synonyms based on icon name patterns
        synonyms = {
            "back": ["previous", "return", "left"],
            "forward": ["next", "right"],
            "up": ["arrow", "top"],
            "down": ["arrow", "bottom"],
            "left": ["arrow", "back"],
            "right": ["arrow", "forward"],
            "add": ["plus", "create", "new", "insert"],
            "remove": ["delete", "minus", "trash", "erase"],
            "edit": ["modify", "change", "update"],
            "save": ["store", "keep", "disk"],
            "open": ["load", "folder"],
            "close": ["exit", "cancel", "x"],
            "search": ["find", "lookup", "magnifier"],
            "settings": ["config", "options", "preferences", "gear"],
            "home": ["house", "main", "start"],
            "calendar": ["date", "time", "schedule"],
            "mail": ["email", "message", "envelope"],
            "phone": ["call", "telephone", "mobile"],
            "camera": ["photo", "picture", "image"],
            "video": ["movie", "film", "play"],
            "music": ["audio", "sound", "song"],
            "volume": ["speaker", "audio", "sound"],
            "wifi": ["wireless", "network"],
            "bluetooth": ["bt", "wireless"],
            "battery": ["power", "charge", "energy"],
            "lock": ["secure", "protect", "key"],
            "unlock": ["open", "unsecure"],
            "cloud": ["sync", "upload", "download"],
            "folder": ["directory", "file"],
            "file": ["document", "page"],
            "print": ["printer", "paper"],
            "share": ["send", "transfer"],
            "copy": ["duplicate", "clone"],
            "cut": ["clip", "scissors"],
            "paste": ["insert", "place"],
            "undo": ["revert", "back"],
            "redo": ["repeat", "forward"],
            "zoom": ["magnify", "scale"],
            "refresh": ["reload", "update", "sync"],
            "favorite": ["star", "like", "bookmark"],
            "pin": ["attach", "fix"],
            "unpin": ["detach", "unfix"],
        }

        name_lower = name.lower()
        for key, values in synonyms.items():
            if key in name_lower:
                keywords.update(values)

        # Add category
        keywords.add(category.lower())

        return sorted(list(keywords))

    def translate_to_chinese(self, name: str, category: str) -> str:
        """
        Translate icon name to Chinese.

        Args:
            name: Icon name
            category: Icon category

        Returns:
            Chinese translation
        """
        # Common translations
        translations = {
            # Navigation
            "GlobalNavButton": "全局导航按钮",
            "Back": "后退",
            "Forward": "前进",
            "Up": "向上",
            "Down": "向下",
            "Left": "向左",
            "Right": "向右",
            "Home": "主页",
            "ChevronLeft": "左箭头",
            "ChevronRight": "右箭头",
            "ChevronUp": "上箭头",
            "ChevronDown": "下箭头",

            # Actions
            "Add": "添加",
            "Remove": "删除",
            "Delete": "删除",
            "Edit": "编辑",
            "Save": "保存",
            "SaveAs": "另存为",
            "Open": "打开",
            "Close": "关闭",
            "Cancel": "取消",
            "Accept": "接受",
            "OK": "确定",
            "Copy": "复制",
            "Cut": "剪切",
            "Paste": "粘贴",
            "Undo": "撤销",
            "Redo": "重做",
            "Refresh": "刷新",
            "Sync": "同步",
            "Share": "分享",
            "Print": "打印",
            "Scan": "扫描",

            # Media
            "Play": "播放",
            "Pause": "暂停",
            "Stop": "停止",
            "Video": "视频",
            "Camera": "相机",
            "Microphone": "麦克风",
            "Volume": "音量",
            "Mute": "静音",
            "Music": "音乐",
            "Audio": "音频",
            "Speaker": "扬声器",

            # Communication
            "Mail": "邮件",
            "Phone": "电话",
            "Message": "消息",
            "Chat": "聊天",
            "Contact": "联系人",
            "People": "人员",
            "Wifi": "无线网络",
            "Bluetooth": "蓝牙",
            "Ethernet": "以太网",
            "VPN": "虚拟专用网",
            "Connect": "连接",

            # Files
            "Folder": "文件夹",
            "File": "文件",
            "Document": "文档",
            "Picture": "图片",
            "Photo": "照片",
            "Calendar": "日历",
            "Library": "库",
            "Download": "下载",
            "Upload": "上传",
            "Cloud": "云端",

            # Status
            "Error": "错误",
            "Warning": "警告",
            "Info": "信息",
            "Success": "成功",
            "Help": "帮助",
            "Flag": "标记",
            "Favorite": "收藏",
            "Like": "喜欢",
            "Dislike": "不喜欢",
            "Block": "阻止",
            "Lock": "锁定",
            "Unlock": "解锁",
            "Shield": "盾牌",

            # UI Elements
            "Settings": "设置",
            "View": "查看",
            "Search": "搜索",
            "Filter": "筛选",
            "Sort": "排序",
            "Zoom": "缩放",
            "ZoomIn": "放大",
            "ZoomOut": "缩小",
            "FullScreen": "全屏",
            "NewWindow": "新窗口",
            "Split": "分割",
            "Pane": "窗格",

            # Devices
            "Devices": "设备",
            "Printer": "打印机",
            "Keyboard": "键盘",
            "Mouse": "鼠标",
            "Touchpad": "触摸板",
            "Webcam": "摄像头",
            "Headphone": "耳机",
            "Headset": "耳麦",
            "Tablet": "平板",
            "Laptop": "笔记本",
            "Desktop": "台式机",
            "Mobile": "手机",
            "Xbox": "Xbox",

            # Others
            "Battery": "电池",
            "Brightness": "亮度",
            "Location": "位置",
            "Map": "地图",
            "Clock": "时钟",
            "Alarm": "闹钟",
            "Timer": "计时器",
            "Calculator": "计算器",
            "Weather": "天气",
            "News": "新闻",
            "Store": "商店",
            "Shop": "购物",
            "Cart": "购物车",
            "Game": "游戏",
            "Health": "健康",
            "Fitness": "健身",
        }

        # Try exact match first
        if name in translations:
            return translations[name]

        # Try to find partial match
        for key, value in translations.items():
            if key.lower() in name.lower():
                return value

        # Return empty string if no translation found
        return ""

    def generate_metadata(self) -> List[Dict[str, Any]]:
        """
        Generate complete icon metadata.

        Returns:
            List of icon metadata dictionaries
        """
        print("Generating icon metadata...")

        icons = []
        validated_count = 0
        missing_count = 0

        for unicode_hex, official_name in sorted(self.official_icons.items()):
            # Convert hex string to int for cmap lookup
            try:
                unicode_int = int(unicode_hex, 16)
            except ValueError:
                missing_count += 1
                print(f"Warning: Invalid unicode {unicode_hex} for {official_name}")
                continue

            # Check if this glyph exists in the font's cmap
            exists_in_font = unicode_int in self.font_cmap

            if not exists_in_font:
                missing_count += 1
                print(f"Warning: {official_name} ({unicode_hex}) not found in font")
                continue

            validated_count += 1

            # Categorize the icon
            category = self.categorize_icon(unicode_hex, official_name)

            # Generate keywords
            keywords = self.generate_keywords(official_name, unicode_hex, category)

            # Generate i18n data
            chinese_translation = self.translate_to_chinese(official_name, category)

            icon = {
                "glyph": f"u{unicode_hex}",
                "unicode": unicode_hex,
                "unicode_string": f"\\u{unicode_hex.lower()}",
                "name": official_name,
                "category": category,
                "keywords": keywords,
                "i18n": {
                    "en": official_name,
                    "zh": chinese_translation
                },
                "verified": True  # Mark as verified from official documentation
            }

            icons.append(icon)

        print(f"Generated metadata for {validated_count} icons ({missing_count} missing from font)")
        return icons

    def save_to_json(self, output_path: str, icons: List[Dict[str, Any]]) -> None:
        """
        Save generated icons to JSON file.

        Args:
            output_path: Path to output JSON file
            icons: List of icon metadata
        """
        output = Path(output_path)
        output.parent.mkdir(parents=True, exist_ok=True)

        # Define categories with enhanced coverage
        categories = [
            {"key": "navigation", "name": "Navigation", "name_zh": "导航", "priority": 1},
            {"key": "actions", "name": "Actions", "name_zh": "操作", "priority": 2},
            {"key": "media", "name": "Media", "name_zh": "媒体", "priority": 3},
            {"key": "communication", "name": "Communication", "name_zh": "通信", "priority": 4},
            {"key": "files", "name": "Files", "name_zh": "文件", "priority": 5},
            {"key": "status", "name": "Status", "name_zh": "状态", "priority": 6},
            {"key": "ui", "name": "UI Elements", "name_zh": "界面", "priority": 7},
            {"key": "devices", "name": "Devices", "name_zh": "设备", "priority": 8},
            {"key": "accessibility", "name": "Accessibility", "name_zh": "辅助功能", "priority": 9},
            {"key": "business", "name": "Business", "name_zh": "商务", "priority": 10},
            {"key": "advanced", "name": "Advanced", "name_zh": "高级", "priority": 11},
            {"key": "specialized", "name": "Specialized", "name_zh": "专用", "priority": 12},
            {"key": "new", "name": "New", "name_zh": "新增", "priority": 13},
            {"key": "uncategorized", "name": "Uncategorized", "name_zh": "未分类", "priority": 999},
        ]

        metadata = {
            "$schema": "./IconMetadata.schema.json",
            "font": {
                "name": self.font['name'].getBestFullName() if self.font else "Segoe Fluent Icons",
                "version": "1.0",
                "copyright": "© 2021 Microsoft Corporation. All Rights Reserved.",
                "source": "Microsoft Official Documentation",
                "generated_by": "create_official_metadata.py"
            },
            "categories": categories,
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
    output_path = project_root / "src" / "UI" / "Lemoo.UI" / "Models" / "Icons" / "IconMetadata.json"

    print("=" * 80)
    print("Segoe Fluent Icons - Official Metadata Generator")
    print("=" * 80)
    print(f"Font: {font_path}")
    print(f"Output: {output_path}")
    print("=" * 80)

    # Generate metadata
    generator = OfficialIconMetadataGenerator(str(font_path))

    try:
        # Step 1: Parse documentation
        generator.parse_documentation()

        # Step 2: Load font and get glyphs
        generator.get_font_glyphs()

        # Step 3: Generate metadata
        icons = generator.generate_metadata()

        # Step 4: Save to JSON
        generator.save_to_json(str(output_path), icons)

        print("\n" + "=" * 80)
        print("[OK] Generation complete!")
        print(f"[OK] Total icons: {len(icons)}")
        print(f"[OK] Output: {output_path}")
        print("=" * 80)

        # Print statistics
        from collections import Counter
        categories = Counter([i['category'] for i in icons])

        print("\nCategory breakdown:")
        for cat, count in sorted(categories.items(), key=lambda x: x[1], reverse=True):
            print(f"  {cat:20s}: {count:4d} icons")

        print("\n" + "=" * 80)

    except Exception as e:
        print(f"\n[ERROR] Error: {e}")
        import traceback
        traceback.print_exc()
        return 1

    finally:
        generator.close()

    return 0


if __name__ == "__main__":
    exit(main())
