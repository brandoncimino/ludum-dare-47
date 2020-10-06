# Play now on Newgrounds - [Babysitting Astronauts](https://www.newgrounds.com/portal/view/767894)!

# Credits:

- [Brandon Cimino](https://github.com/brandoncimino)
- Michael
- David
- [Nicole Aretz](https://github.com/nicolearetz)

[Design Sketchbook](https://docs.google.com/document/d/1REFAH6utFzSfryoI7g7DsX0OYxLwfHTh9KK1VVgZbfk/edit?usp=sharing)


## Problems that arose during the Build process:

|Problem|Explanation|Solution|
|-------|-----------|--------|
Build fails to compile due to invalid `NUnit` references | [BrandonUtils](https://github.com/brandoncimino/brandon-utils) referenced NUnit in 1 Runtime file (`SaveData.cs`), which is apparently not allowed. NUnit was used because of their nice assertion syntax, but this is, [according to the NUnit devs](https://stackoverflow.com/a/46852617), bad practice, so it needed to be removed anyways. | Replacing the NUnit assertion with explicit `throws`. 
Build fails to compile due to invalid `NUnit` references in `com.fowlfever.brandonutils.tests` | The `.tests.asmdef`, which referenced `NUnit`, was being built on _any_ platform. | Restrict `.tests.asmdef` to only the `Editor` platform.
Build fails to compile due to invalid `NUnit` references in `com.fowlfever.brandonutils.runtime.testing` | The `BrandonUtils.Runtime.Testing` ~~package~~ namespace contained `NUnit` extensions, custom assertions, etc. | Move the ~~package's~~ namespace's contents to the `BrandonUtils.Editor.Testing` package, which, in addition to being "editor specific", for these purposes is also "non-production". <br/><br/>**NOTE:** Rider doesn't understand the folder structure for Unity packages, and wants to change the namespace from `Packages.BrandonUtils.Editor.Testing` to just `Testing`. This has the side-effect of disallowing the use of Rider's "Move" refactoring (because it forces "proper" namespaces), which means that I had to manually fix the `using` directives throughout the project afterwards.
Unable to compile `BrandonUtils`'s `[EditorInvocationButton]` attribute | The `[EditorInvocationButton]` attribute is, as expected, only compiled for the `Editor` platform - which causes issues because it is referenced by `runtime` code. | Comment out `[EditorInvocationButton]` calls. <br/><br/>**NOTE/TODO:** There _should_ be a correct way to do this, since Unity has built-in editor-specific attributes like `[Header]` and `[Range]` that compile perfectly fine.
WebGL (HTML5) build fails to load _at all_ when run locally (i.e. by clicking the build's `index.html`) | In Chrome, this is due to security restrictions ([WebGL Not Loading on Chrome](https://gamedev.stackexchange.com/questions/114239/unity-webgl-not-loading-on-chrome)). However, Firefox also doesn't work, so who knows. | Give up and move on. Plus, the Unity documentation for [Building and running a WebGL project for Unity 2019.4](https://docs.unity3d.com/2019.4/Documentation/Manual/webgl-building.html) (which is what you are directed to when you [don't specify a version](https://docs.unity3d.com/Manual/webgl-building.html)) matches the Unity version we used, `2020.1.7f1`, but the documentation for `2020.1` appears to be [absolute nonsense](https://docs.unity3d.com/2020.1/Documentation/Manual/webgl-building.html).
WebGL (HTML5) build hangs indefinitely at ~90% loaded when hosted through Newgrounds | No error is actively displayed to the user, but the javascript console reveals <c>`Uncaught ReferenceError: unityFramework is not defined at HTMLScriptElement.script.onload (WebGL)`</c> | This is a [known, supposedly-fixed issue](https://forum.unity.com/threads/uncaught-referenceerror-unityframework-is-not-defined-at-htmlscriptelement-script-onload-webgl.803967/) caused by compression. The solution discussed on that Unity forum thread solved the issue: Go to `Project Settings` -> `Player` -> `Settings for WebGL` -> `Publishing Settings` -> `Compression Format`, and set it to `Disabled`. (This brought the zipped build from `~11mb` to `~13mb`)
