# Description

`Macrocosmic` is a procedural 2D galaxy generator made with Unity.

<figure>
<img src="https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExaHQ5YjlrMzZ6OWRjbmxubHJuaHhtbGVyNnZ2bm5kbm9zYm00emY4ZiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/wH9O3Ly6ZVmw3I42Jj/giphy.gif" alt="A gif previewing the generation menu" width="600" height="300"/>
</figure>

# Features

All interactions are made with `w`, `a`, `s`, `d`, `esc`, and interacting with the mouse.

### ★ Galaxy generation

- Plenty of parameters to generate a wide variety of spiral, elliptical, and ring galaxies.

<figure>
<img src="https://media4.giphy.com/media/v1.Y2lkPTc5MGI3NjExaDVicmNuZGd0OGZiOXpsYWt5OTRuOWRuczFiamRtYmV2bXg5ZHg1ZyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/wA1OXNlzaZ4xW79B9r/giphy.gif" alt="A gif previewing the generation menu" width="600" height="300"/>
</figure>

### ★ Solar systems

- Nine different planet types are generated based on distance, where hot planets are found closer to their star.
- The camera will lock onto orbiting planets to bring you along their journey around the star.

<figure>
<img src="https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExeXI4NTc5Y2Z5eThhbWZpZmxwbGVhZzgyZGN3aHA2a3lyZGs3d3U2OCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/8BfH29N7xNwPD2Mp8i/giphy.gif" alt="A gif previewing solar systems" width="600" height="300"/>
</figure>

### ★ Calendar

- The calendar's passing of time can be adjusted to speed up or slow down orbiting planets.

<figure>
<img src="https://media2.giphy.com/media/v1.Y2lkPTc5MGI3NjExd2IzOWpqbmo1cjlza2c0M3lsbWx1ODRwZjFoZmtoMWcyNnl3cTQ1MyZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/DYgkeZHKLAVjW5ay6E/giphy.gif" alt="A gif previewing the calendar's passing of time" width="600" height="300"/>
</figure>

### ★ Save and load

- Save the current state of your favorite galaxy!

<figure>
<img src="https://media3.giphy.com/media/v1.Y2lkPTc5MGI3NjExOXprM3BkcHNyajVqMTRyMzVmbGNidDFrdnEyb3NqcHNob3B4Z2tnaiZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/zH56P13NclC9kLtaQ2/giphy.gif" alt="A gif previewing the save and load menu" width="600" height="300"/>
</figure>

### ★ Summary

- The sleek summary screen will tell you all you need to know about your generated galaxy.

<figure>
<img src="https://media1.giphy.com/media/v1.Y2lkPTc5MGI3NjExMXgyNTlhYmw1N29rdjB4c2YydmRsbWF6cXk4NTRrbmNmOXpuNGZieSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/41xefKPWYBgLNbpJkL/giphy.gif" alt="A gif previewing the summary menu" width="600" height="300"/>
</figure>

# Build

Unity version `2020.3.32f1` is required to build the project. `Macrocosmic` has only been built and tested for Windows.

# Notes

In addition to procedural generation, `Macrocosmic` is also an exercise in providing a pleasurable user experience. For example, this project was my first concerted effort to create a visually pleasing graphical user interface. Furthermore, a great deal of effort was made to beautify the galaxy's appearance. I settled on using Perlin noise to generate textures which are rendered as billboard particles across the galaxy. Given my lack of shader scripting knowledge during the development of `Macrocosmic`, all eye candy is achieved using particles or Shader Graph.
