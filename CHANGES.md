# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [0.11.0] - 2024-11-xx
### Added
- Experimental Apple Vision Pro and iOS support
- More scalability options
- Added true multiplatform prefabs
- Added build profiles for Android (Flat, OpenXR, Meta) and Windows (OpenXR, Meta)

### Changed
- Folder structure updated to match current standard
- Updated to Unity 6
- Using Forward+ and Resident GPU drawer by default
- Tweaked the load balancer to makes the game faster to load
- The Game scene already contains many prefabs in the scene to improve load times

## [0.10.0] - 2020-xx-xx
### Added
- Missing Records
- Initial work for NPC support
- Vulkan support for Oculus Quest
- New Unity Input System
- Started to work on the UI system
- Day night system enhancement
- Player management (in progress)
### Changed
- Universal Render Pipeline integration
### Removed
- Oculus AssetStore Package
- HTC Vive Focus Support
- Vive Focus and OSVR support
- Legacy Renderer support

## [0.9.0] - 2019-xx-xx
### Added
- Android (Touch) Support
- Oculus Go Support
- Vive Focus Support
- HDRP Support
- Main menu with options
- New Input Manager
### Changed
- The config.ini file is deprecated

## [0.8.0] - 2018-03-29
### Added
- Initial Teleportation system (still WIP)
### Changed
- Improved performances a LOT by exposing three new variables
### Fixed
- Builds start in VR directly 
- Fixed the UI in VR

## [0.7.0] - 2018-03-27
### Added
- Lightweight Render Pipeline for better performances
- New exposed variables for more performances
- Use Vulkan as first GFX API on Linux
### Changed
- Updated to Unity 2018.1b12
- Improved performances
### Fixed
- Specular value is set to 0.25 instead of 0.5

## [0.6.0] - 2018-03-14
### Added
- New VR integration
- Controller support
- Auto Normal Map generation
- Using the Post processing stack V2
- Normal Map support
- Day/Night Cycle
- Graphics Improvement
### Changed
- Updated to Unity 2018.1b10
### Removed
- OSVR Support

## [0.5.0] - 2016-10-21
### Added
- OSVR Support
- Other VR SDKs Support (not yet enabled)
- Enhanced VR support for UI and HUD

## [0.4.0] - 2016-10-18
### Added
- New flags to enable new features (see readme.md)
- Experimental static creatures support
- Experimental weapon support (with home made animation)
- Books and scrolls support
- Crosshair
- Morrowind cursor
### Changes
- Use the same keys binding as Morrowind
### Fixes
- Fixed an encoding issue for texts with accents

## [0.3.0] - 2016-10-09
### Added
- More parameters in config.ini
- Post Effects: Ambient Occlusion, AntiAliasing, Bloom, UnderWater
- Move in the head direction (VR)

## [0.2.0] - 2016-10-05
### Added
- Possibility to keep the Morrowind's folder path
- Configuration file
- New material system (Standard, Bumped, Unlit, MWMaterial)
### Fixes
- Few fixes here and there

## [0.1.0] - 2016-10-02
### Added
- VR Support (Oculus Rift / OpenVR)
- VR UI
- Gamepad support
### Fixes
- Fixed the MWMaterial