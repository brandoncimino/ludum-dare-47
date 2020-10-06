# Play now on Newgrounds - [Babysitting Astronauts](https://www.newgrounds.com/portal/view/767894)!

# ludum-dare-47

# [Design Sketchbook](https://docs.google.com/document/d/1REFAH6utFzSfryoI7g7DsX0OYxLwfHTh9KK1VVgZbfk/edit?usp=sharing)

# Credits:

- [Brandon Cimino](https://github.com/brandoncimino)
- Michael
- David
- [Nicole Aretz](https://github.com/nicolearetz)



## Problems that arose during the Build process:
(Solutions will be written up later)
- BrandonUtils referenced NUnit in 1 Runtime file, which is not allowed
- BrandonUtils had .tests.asmdef (which referenced NUnit) being included in ANY platform (not just the Editor)
- BrandonUtils custom attribute for EditorInvocationButton couldn't compile because it was editor-only
- Can only build for mac on mac
