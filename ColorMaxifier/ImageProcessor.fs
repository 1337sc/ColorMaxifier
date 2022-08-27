module ImageProcessor
open System.Drawing
open System.Collections.Generic
open System.IO

type ImageProcessor(image: Bitmap, minColorValue: int, maxColorValue: int, minRedValue: int, maxRedValue: int, minGreenValue: int, maxGreenValue: int, minBlueValue: int, maxBlueValue: int, minColor: Color, maxColor: Color) = 
    do printfn "Creating image w/: \n\tminColorValue: %d\n\tmaxColorValue: %d\n\tminRedValue: %d\n\tmaxRedValue: %d\n\tminGreenValue: %d\n\tmaxGreenValue: %d\n\tminBlueValue: %d\n\tmaxBlueValue: %d\n\tminColor: %s\n\tmaxColor: %s" minColorValue maxColorValue minRedValue maxRedValue minGreenValue maxGreenValue minBlueValue maxBlueValue (minColor.ToString()) (maxColor.ToString())
    let innerImage = image
    static let imageExtensions = [".JPG"; ".JPEG"; ".JPE"; ".BMP"; ".GIF"; ".PNG"]
    
    let getMeanColor() =
        let pixelCount = innerImage.Height * innerImage.Width
        let mutable meanAlpha = 0
        let mutable meanRed = 0
        let mutable meanGreen = 0
        let mutable meanBlue = 0

        for i in 0..innerImage.Width - 1 do 
            for j in 0..innerImage.Height - 1 do
                let pixel = innerImage.GetPixel(i, j)
                meanAlpha <- meanAlpha + (pixel.A |> int32)
                meanRed <- meanRed + (pixel.R |> int32)
                meanGreen <- meanGreen + (pixel.G |> int32)
                meanBlue <- meanBlue + (pixel.B |> int32)

        Color.FromArgb(meanAlpha / pixelCount, meanRed / pixelCount, meanGreen / pixelCount, meanBlue / pixelCount)
                
    let isColorBrighter(firstColor: Color, secondColor: Color) = 
        firstColor.R > secondColor.R || firstColor.G > secondColor.G || firstColor.B > secondColor.B
    
    member this.saveMaxifiedByMeanColor(filename: string) = 
        let resultImage = new Bitmap(image.Width, image.Height)
        let meanColor = getMeanColor()

        for i in 0..innerImage.Width - 1 do 
            for j in 0..innerImage.Height - 1 do
                let pixel = innerImage.GetPixel(i, j)
                if isColorBrighter(pixel, meanColor) then 
                    resultImage.SetPixel(i, j, maxColor)
                else
                    resultImage.SetPixel(i, j, minColor)

        resultImage.Save(filename)

    member this.saveAddedByMeanColor(filename: string) = 
        let resultImage = new Bitmap(image.Width, image.Height)
        let meanColor = getMeanColor()

        for i in 0..innerImage.Width - 1 do 
            for j in 0..innerImage.Height - 1 do
                let pixel = innerImage.GetPixel(i, j)
                if isColorBrighter(pixel, meanColor) then 
                    resultImage.SetPixel(i, j, Color.FromArgb(pixel.A |> int32, 
                        ((pixel.R |> int32) + maxColorValue) % 256, 
                        ((pixel.G |> int32) + maxColorValue) % 256, 
                        ((pixel.B |> int32) + maxColorValue) % 256))
                else
                    resultImage.SetPixel(i, j, Color.FromArgb(pixel.A |> int32, 
                        ((pixel.R |> int32) + minColorValue) % 256, 
                        ((pixel.G |> int32) + minColorValue) % 256, 
                        ((pixel.B |> int32) + minColorValue) % 256))

        resultImage.Save(filename)
    
    member this.saveAddedToEachColor(filename: string) = 
        let resultImage = new Bitmap(image.Width, image.Height)
        let meanColor = getMeanColor()

        for i in 0..innerImage.Width - 1 do 
            for j in 0..innerImage.Height - 1 do
                let pixel = innerImage.GetPixel(i, j)
                if isColorBrighter(pixel, meanColor) then 
                    resultImage.SetPixel(i, j, Color.FromArgb(pixel.A |> int32, 
                        ((pixel.R |> int32) + maxRedValue) % 256, 
                        ((pixel.G |> int32) + maxGreenValue) % 256, 
                        ((pixel.B |> int32) + maxBlueValue) % 256))
                else
                    resultImage.SetPixel(i, j, Color.FromArgb(pixel.A |> int32, 
                        ((pixel.R |> int32) + minRedValue) % 256, 
                        ((pixel.G |> int32) + minGreenValue) % 256, 
                        ((pixel.B |> int32) + minBlueValue) % 256))

        resultImage.Save(filename)

    static member isImage(file: FileInfo) = 
        List.contains (file.Extension.ToUpper()) imageExtensions

    new(image: Bitmap, minColorValue: int, maxColorValue: int) = ImageProcessor(image, minColorValue, maxColorValue, 0, 256, 0, 256, 0, 256, Color.Black, Color.White)
    new(image: Bitmap, minColor: Color, maxColor: Color) = ImageProcessor(image, 0, 256, 0, 256, 0, 256, 0, 256, minColor, maxColor)
    new(image: Bitmap, minRedValue: int, maxRedValue: int, minGreenValue: int, maxGreenValue: int, minBlueValue: int, maxBlueValue: int) = ImageProcessor(image, 0, 256, minRedValue, maxRedValue, minGreenValue, maxGreenValue, minBlueValue, maxBlueValue, Color.Black, Color.White)
    new(image: Bitmap) = ImageProcessor(image, 0, 256, 0, 256, 0, 256, 0, 256, Color.Black, Color.White)