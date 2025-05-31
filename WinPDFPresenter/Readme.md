# WinPDFPresenter - Presenter View for PDFs on Windows

Ever thought about presenting your LaTeX/PDF presentations on Windows? Wanted to include presenter notes and a timer just like in PowerPoint?
WinPDFPresenter is your friend :)

Open PDFs (and their corresponding pdfnotes-files) and have a presentation window and a seperate presenter view, where you'll see
the current slide, the next and previous slides, the current timer and your presenter notes for the current slide.

When "edit pdfnotes-file" is selected, you are able to generate/edit your pdfnotes-file for a presentation. Type in your markdown notes 
and see the notes directly rendered in one screen. Go back and forth between slides with the press of just a button. When you save your
file, edit the duration and the last minutes of your presentation.

## Timer

There are two points of interest here:

1. Duration<br /> Sets the overall amount of time you have for the presentation. When the timer runs out, you get a red text saying "TIME'S UP."
2. Last Minutes<br />During the last minutes, the timer will switch colours to an amber colour in order to warn you that your talk will be over soon.

## Hotkeys

|     **Key**     | **Action (ONLY IN PRESENTATION MODE)**        |     **Windows**     |
|:---------------:|-----------------------------------------------|:-------------------:|
|       F11       | Make presentation full-screen                 |   **All windows**   |
|       F5        | Start Presentation (full-screen + Timer)      | Only Presenter View |
|     Shift+S     | Start Timer                                   | Only Presenter View |
|     Shift+P     | Pause Timer                                   | Only Presenter View |
|     Shift+R     | Reset Timer (does not start the timer again!) | Only Presenter View |
|        B        | Make presentation view go blank               | Only Presenter View |
| D / -> / PgUp   | Go to next slide                              | Only Presenter View |
| A / <- / PgDown | Go to previous slide                          | Only Presenter View |