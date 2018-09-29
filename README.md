# TalkAbout
A text-to-speech Windows 10 Store app.

Designed for people who have no natural speech, or whose natural speech can be difficult for others to understand, TalkAbout allows typed messages to be spoken out loud in order to support face-to-face communication.

TalkAbout is an example of Augmentative and Alternative Communication ([AAC][1]).

## Current features

1. Save phrases, either in categories or in a single list, to retrieve quickly and easily at another time.
2. Display saved phrases in alphabetical order, by most recently used, by most frequently used, or by category.
3. Filter your saved phrases as you type, allowing you to reach the desired save phrase more quickly.
4. TalkAbout can be used purely with the keyboard; no pointing device required (although it works with pointing devices as well).
5. Save abbreviations, which expand to full phrases when typed (for example, type 'hha' and a space, which expands to 'hello how are you').
6. Save pronunciations, which tell the synthetic voice how to pronounce a word if it can't pronounce it correctly by default (useful for the names of some places and people, for example).
7. Customise the interface of the app.  Don't need to be able to change the order of your saved phrases? You can switch certain buttons on and off.

## Possible future developments

TalkAbout is a work in progress.  Some possible future developments include:

1. Integration with alternative synthetic voice vendors.  At the moment, TalkAbout works with the synthetic voices built into Windows 10 (note: this is not the same as SAPI5).  Integration with other vendors would allow greater choice and personalisation.
2. History of phrases.  The ability to recall previously spoken phrases (without having to explicitly save them) would provide further flexibility in terms of generating content.
3. Integrated dwell select, for selecting with a pointer device without having to physically click.

## Technical details

TalkAbout is implemented in C# and XAML. 

## Screenshots
### Screenshot 1: Minimal buttons 
The chat screen with minimal buttons, with saved phrases organised in a single list without categories.

![minimum buttons][screenshot1]

### Screenshot 2: All buttons on
The chat screen with all of the buttons turned on, with saved phrases organised in categories.  Buttons and other controls are also displaying accessibility shortcuts.

![all buttons][screenshot2]

[1]: https://www.communicationmatters.org.uk/page/about-aac "Communication Matters: About AAC"
[screenshot1]: screenshots/chat_min_buttons.png "The chat screen with minimal buttons"
[screenshot2]: screenshots/chat_all_buttons.png "The chat screen with all buttons"
