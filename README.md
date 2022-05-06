# ECARules4All

Code repository for the implementation of the ECARules4All project

ECARules4All is a Unity package that enables users without coding knowledge to setup Virtual Reality worlds through Event-Condition Action rules (ECA), defined in natural language. The solution is based on templates that can be configured for creating the users' own experiences. The rules define the behaviour of each virtual object in the environment in isolation and its interaction with other objects. Once equipped with components supporting the runtime specification of behaviour rules, the XR environments offer a high degree of customization to end-users.

For more information refer to the following link: https://cg3hci.dmi.unica.it/lab/en/projects/ecarules4all

## Installation

### Prerequisites

- The following packages installed from the Package manager:
  - XR Interaction Toolkit;
  - Universal RP;
  - Oculus XR Plugin (for using Oculus Headsets).

### Configuration

- Download the ECARules4All prefab from the [Releases](https://github.com/cg3hci/ECARules4All/releases) section of this repository;
  - You can also download the `documentation.chm` file, which contains the documentation for the components and scripts of the package.
- Set `Edit > Project Settings > Player > Settings for PC, Mac & Linux Standalone > Expand "Other Settings" > Api Compatibility Level => .NET 4.x`;
- Check `Edit > Project Settings > XR Plug-In Management > Plug-in Providers > "Oculus"`;
- Import all the files from the ECARules4All package into your project;
- Import the ECAKit prefab into your scene (it can be found in `Assets > Prefabs > ECAKit.prefab`).