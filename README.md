# ColorMaxifier
Working with images, videos and colors, just for fun

# ImageProcessor
all ImageProcessor functions work related to mean colors found for single images.
- **saveMaxifiedByMeanColor**: paints the pixels that are brighter than the mean color with **maxColor** and the darker ones with **minColor**.
- **saveAddedByMeanColor**: add **maxColorValue** to each color of the pixels that are brighter than the mean color and **minColorValue** to the pixels that are darker than the mean color.
- **saveAddedToEachColor**: add **min** or **max** value to the corresponding Red, Green and Blue values of the colour of each pixels

# VideoProcessor
used for creating videos from frames and vice versa 
- **videoFromFrames**: create video from frames, supports regexp patterns
- **framesFromVideo**: extract frames from video
