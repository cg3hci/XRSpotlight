# XRSpotlight

Code repository for the implementation of the XRSpotlight project

XRSpotlight is a Unity editor that enables users novice developers by curating a list of the XR interactions defined in a Unity scene and presenting them as rules in natural language. The support provided by XRSpotlight consists of three main features: (1) expressing interactions in natural language, (2) finding examples of similar interactions in the scene, and (3) copy-pasting interactions in a toolkit-agnostic way.

## Installation

### Prerequisites

- The following packages installed:
  - An XR Interaction Toolkit (MRTK 2.7 or SteamVR)
  - Universal RP;

### Configuration

- Download the ECARules4All prefab from the [Releases](https://github.com/cg3hci/ECARules4All/releases) section of this repository;
  - You can also download the `documentation.chm` file, which contains the documentation for the components and scripts of the package.
- Set `Edit > Project Settings > Player > Settings for PC, Mac & Linux Standalone > Expand "Other Settings" > Api Compatibility Level => .NET 4.x`;
- Check `Edit > Project Settings > XR Plug-In Management > Plug-in Providers > "Oculus"`;
- Import all the files from the ECARules4All package into your project;
- Import the ECAKit prefab into your scene (it can be found in `Assets > Prefabs > ECAKit.prefab`).
