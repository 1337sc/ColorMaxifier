open ImageProcessor
open System.Drawing
open System.IO
open System
open System.Collections.Generic
open VideoProcessor

let inputFolder = "./input"
let outputFolder = "./output"
let frameExtension = ".png"
let videoExtension = ".mp4"

let directory = new DirectoryInfo(Path.GetFullPath(inputFolder))

if Directory.Exists(Path.GetFullPath(outputFolder)) |> not then
    Directory.CreateDirectory(Path.GetFullPath(outputFolder)) |> ignore

let files = directory.GetFiles()
for fNum in 0..files.Length - 1 do
    let f = files[fNum]
    let fName = f.Name.Replace(f.Extension, String.Empty)
    let mutable c = 0
    for i in 128..16..256 do
        for j in 128..16..256 do
            task {
                let imageProcessor = new ImageProcessor(new Bitmap(Bitmap.FromFile(f.FullName)), 
                    Color.FromArgb(255, i, i, j), 
                    Color.FromArgb(255, j, j, i))
                let newFilePath = Path.GetFullPath(Path.Combine(outputFolder, 
                    fName + c.ToString().PadLeft(5, '0') + frameExtension))
                c <- c + 1
                imageProcessor.saveMaxifiedByMeanColor(newFilePath)
            }
            |> Async.AwaitTask
            |> ignore

    let outputFile = new FileInfo(Path.GetFullPath(Path.Combine(outputFolder, fName + videoExtension)))
    if outputFile.Exists then
        outputFile.Delete()
    let videoProcessor = new VideoProcessor(Path.GetFullPath("ffmpegPath"))
    videoProcessor.videoFromFrames(5, Path.GetFullPath(Path.Combine(outputFolder, fName + "%05d" + frameExtension)), 
        Path.GetFullPath(Path.Combine(outputFolder, fName + videoExtension)))