# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Releases]

### [2.2.2] - 2021-06-13
#### Changed
* Modified internal parallelization for Roslyn Project parsing to use async correctly.
* Modified plugin loading so that by default plugin assemblies that are not in-sync with the nuget package assemblies in use by Genesis.CLI will issue a warning and:
    * If on the same major and minor version will still be loaded.
    * If on an older major or minor version will not be loaded. These can still be force-loaded if a user passed the `load-unsafe` argument.

#### Added
* The `CHANGELOG.MD` file containing release notes is now included with the Unity package.
* Created Unity flow for third-party developers to easily expose their plugins for other users and easily install or update them in their Genesis.CLI configuration.
* Added new Genesis.CLI argument for `load-unsafe` to force load any out-of-date plugins, if any.

### [2.2.1] - 2021-06-06
#### Added
* Added additional helper methods and caching to NamedTypeSymbolInfo. All members are now lazily-loaded rather than initialized in constructor.

#### Fixed
* Modified AutoUpdate functionality so that the zip extraction only overwrites content from the zip file (does not disturb other existing custom plugins for example).

### [2.1.1] - 2021-06-05
#### Added
* Added new overload of `AssembliesConfig.FilterTypeSymbols` method that accepts a collection of `NamedTypeSymbolInfo`.

#### Fixed
* Fixed bug in `ITypeSymbol.GetArrayElementType` where the element type attempted to be returned would occasionally result in a cast exception from being cast to a `INamedTypeSymbol` when the return type is a `ITypeSymbol`.

### [2.1.0] - 2021-06-05
#### Added
* Created NamedTypeSymbolInfo as a wrapper for `INamedTypeSymbol` that caches commonly used member returns from the symbol. It's cached fields and helper methods can be used over directly accessing `INamedTypeSymbol` to improve CPU perf.
* Added auto-update detection for when a Genesis.CLI installation is out-of-date when compared to the Genesis package version. 

#### Changed
* Modified MemoryCache to cache a collection of `NamedTypeSymbolInfo` instead of `INamedTypeSymbol`. Adjusted usage in existing plugins. 
* Replaced menu item to select `Genesis.CLI.zip` file with updating existing installation, if present.
* Parallelized several loops for CPU perf improvement.

## CL Template
This template should be used for any new releases to document any changes.

### [0.0.0] - 1900-01-01
#### Added
For new features.

#### Changed
For changes in existing functionality.

#### Deprecated
For soon-to-be removed features.

#### Removed
for now removed features.

#### Fixed
For any bug fixes.

#### Security
In case of vulnerabilities.