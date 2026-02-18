# Description

`Macrocosmic` is a procedural 2D galaxy generator made with Unity.

![A gif previewing the save and load menu](https://raw.githubusercontent.com/jakemikepete/media/main/macrocosmic/generate_random.gif)

# Features

All interactions are made with `w`, `a`, `s`, `d`, `esc`, and interacting with the mouse.

### ★ Galaxy generation

- Plenty of parameters to generate a wide variety of spiral, elliptical, and ring galaxies.

![A gif previewing the save and load menu](https://raw.githubusercontent.com/jakemikepete/media/main/macrocosmic/generate_menu.gif)

### ★ Solar systems

- Nine different planet types are generated based on distance, where hot planets are found closer to their star.
- The camera will lock onto orbiting planets to bring you along their journey around the star.

![A gif previewing the save and load menu](https://raw.githubusercontent.com/jakemikepete/media/main/macrocosmic/solar_system.gif)

### ★ Calendar

- The calendar's passing of time can be adjusted to speed up or slow down orbiting planets.

![A gif previewing the save and load menu](https://raw.githubusercontent.com/jakemikepete/media/main/macrocosmic/calendar.gif)

### ★ Save and load

- Save the current state of your favorite galaxy!

![A gif previewing the save and load menu](https://raw.githubusercontent.com/jakemikepete/media/main/macrocosmic/load.gif)

### ★ Summary

- The sleek summary screen will tell you all you need to know about your generated galaxy.

![A gif previewing the save and load menu](https://raw.githubusercontent.com/jakemikepete/media/main/macrocosmic/summary.gif)

# Build

Unity version `2020.3.32f1` is required to build the project. `Macrocosmic` has only been built and tested for Windows.

# Notes

In addition to procedural generation, `Macrocosmic` is also an exercise in providing a pleasurable user experience. For example, this project was my first concerted effort to create a visually pleasing graphical user interface. Furthermore, a great deal of effort was made to beautify the galaxy's appearance. I settled on using Perlin noise to generate textures which are rendered as billboard particles across the galaxy. Given my lack of shader scripting knowledge during the development of `Macrocosmic`, all eye candy is achieved using particles or Shader Graph.
