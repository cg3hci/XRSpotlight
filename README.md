# XRSpotlight

Code repository for the implementation of the XRSpotlight project

XRSpotlight is a Unity editor that enables users novice developers by curating a list of the XR interactions defined in a Unity scene and presenting them as rules in natural language. The support provided by XRSpotlight consists of three main features: 
- expressing interactions in natural language,  
- finding examples of similar interactions in the scene
- copy-pasting interactions in a toolkit-agnostic way.

## Installation

### Prerequisites

- The following packages installed:
  - An XR Interaction Toolkit (MRTK 2.7 or SteamVR)
  - Universal RP;
  - Unity version 2020.3.35f1

### Configuration

- Download the source code of this repository;
  - The project is already configured with MRTK, you can find an example scene
- Open `Edit > Project Settings > Player > Other Settings` and add "MRTK" or "SteamVR" according to the Interaction toolkit you are using, then click to Apply
- Here, you may need to regenerate the project files. To do so, go to `Edit > Preferences > External Tools` and press the button "Regenerate Project Files"
- In the Unity panel, open `Window > UI Toolkit > XRSpotlight` 
- Now you can see the current scene information with XRSpotlight;
- If you witsh to include XRSpotlight in your own project, you can copy the "Editor" folder of the repository. 
