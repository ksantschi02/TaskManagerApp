A test of WPF and .NET capabilities by creating a small standalone To-Do List Desktop Widget for personal usage. Free for personal usage, not for commercial distribution.

Features by version:
1.0
- Can add tasks to to-do list
- Can check off items, which will fade out and be removed from the list
- Widget stays pinned on top, allowing user to interact with other applications while the to-do list is in view
- List is saved whenever the widget is closed, and loads back up upon restarting
- Button to clear list
- Dynamic window title for number of tasks remaining

I may add further functionality later on, but that is yet to be determined.
List of features I may implement in the future:
- Drag and drop ordering
- Tab system so you can have multiple lists
- Possibly also name the tabs
- Text wrapping (doesn't work quite so well at the moment)
- Figure out how to minimize to system tray (might be a Windows feature that I haven't figured out)
- Test saving feature. If I hard shut down my computer, will likely not save my to do list. One solution would be to just have it save every time an item is added to/checked off of the list.
- Possible web-based feature. Have user sign into google account, save to do list as csv to drive. For this purpose:
  - Create second application for phone that communicates with same csv file. Allows user to add/check off items from phone, saves and updates over web.
  - If possible, create interactive lock screen background that shows to do list
