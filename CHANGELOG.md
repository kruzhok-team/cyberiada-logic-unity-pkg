# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

<!-- Headers should be listed in this order: Added, Changed, Deprecated, Removed, Fixed, Security -->

## [0.2.5] - 17.02.2025
### Added
 - New API allowing to copy GraphDocument in single action both changing ID and keeping original one
### Fixed
 - Equalitiy condition fix

## [0.2.4] - 31.01.2025
### Added
 - Added ID property to INodeView and IEdgeView API
 - Added API to retrieve Node- and Edge- views by their ID in EditorCore

## [0.2.3] - 27.12.2024
### Fixed
 - Exceptions if no metadata is present
 - Proped nested node duplication
 - Error on node removal

## [0.2.2] - 12.12.2024
### Added
 - Basic documentation
 - Edge preview management API
 - Added define-hidden classes like Vector2 to allow compiling outside of Unity Engine
 
## [0.2.1] - 22.11.2024
### Added
 - Metadata block in resulting file
 - Format tag in resulting file
 
## [0.2.0] - 02.11.2024
### Added
 - Multiple parameters support
 - Node duplication API
 - Initial node reconnecting API
 - Base graph reference storing
 - Graph name storing
### Fixed
 - Various spec compability fixes

## [0.1.6] - 19.09.2024
### Added
 - Added Condition to Node

## [0.1.5] - 06.08.2024
### Changed
 - IEntity now implements IExecutionContextSource directly

## [0.1.4] - 29.07.2024
### Changed
 - Removed unnecessary logs output

## [0.1.3] - 18.06.2024
### Added
 - Flag to element creation factories API to check if its implied to place new element automatically in layout or not

## [0.1.2] - 11.06.2024
### Added
 - Ability to copy graph via Graph.GetCopy method
### Fixed
 - HSM generation now properly treats edges in root graph connecting nested nodes

## [0.1.1] - 03.06.2024
### Changed
 - Variable getters are now generic
 
## [0.1.0] - 25.05.2024
### Added
 - GraphBase
 - Cyberiada-GraphML Extension
 - GraphEditorCore
 - LogicBridge
 - Graph based HSM interpretator