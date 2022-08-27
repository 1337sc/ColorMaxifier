open ImageProcessor
open System.Drawing
open System.IO
open System
open System.Collections.Generic
open VideoProcessor

let inputFolder = "./input"
let outputFolder = "./output"

let generatedFrameExtension = ".png"
let generatedVideoExtension = ".mp4"

let videoProcessor = new VideoProcessor(Path.GetFullPath("ffmpeg path"))

let inputDirectory = new DirectoryInfo(Path.GetFullPath(inputFolder))
let outputDirectory = new DirectoryInfo(Path.GetFullPath(outputFolder))

if Directory.Exists(Path.GetFullPath(outputFolder)) |> not then
    Directory.CreateDirectory(Path.GetFullPath(outputFolder)) |> ignore

let createVideoFromFrames(fName: string, framesPattern: string) = 
    let outputFile = new FileInfo(Path.GetFullPath(Path.Combine(outputFolder, fName + generatedVideoExtension)))
    if outputFile.Exists then
        outputFile.Delete()
    videoProcessor.videoFromFrames(30, Path.GetFullPath(Path.Combine(outputFolder, framesPattern)), 
        outputFile.FullName)

let processImage(f: FileInfo) =
    let fName = f.Name.Replace(f.Extension, String.Empty)
    let mutable c = 0
    let rnd = new Random()
    for i in 0..8..255 do
        task {
            let imageProcessor = new ImageProcessor(new Bitmap(Bitmap.FromFile(f.FullName)), Color.FromArgb(255, i, i, i), Color.FromArgb(255, 255 - i, 255 - i, 255 - i))
            let newFilePath = Path.GetFullPath(Path.Combine(outputFolder, 
                fName + c.ToString().PadLeft(5, '0') + generatedFrameExtension))
            c <- c + 1
            imageProcessor.saveMaxifiedByMeanColor(newFilePath)
        }
        |> Async.AwaitTask
        |> ignore

    createVideoFromFrames(fName, fName + "%05d" + generatedFrameExtension)

let processVideo(f: FileInfo) = 
    let fName = f.Name.Replace(f.Extension, String.Empty)
    videoProcessor.framesFromVideo(f.FullName, Path.GetFullPath(Path.Combine(outputFolder, fName + "%05d" + generatedFrameExtension)))
    let frames = outputDirectory.GetFiles(fName + "*" + generatedFrameExtension)
    for c in 0..frames.Length - 1 do
        let frame = frames[c]
        task {
            let imageProcessor = new ImageProcessor(new Bitmap(Bitmap.FromFile(frame.FullName)), Color.FromArgb(0x56, 0x56, 0xa5), Color.FromArgb(0xed, 0xed, 0x9a))
            let newFilePath = Path.GetFullPath(Path.Combine(outputFolder, 
                fName + "worked_" + c.ToString().PadLeft(5, '0') + generatedFrameExtension))
            imageProcessor.saveMaxifiedByMeanColor(newFilePath)
        }
        |> Async.AwaitTask
        |> ignore
        
    createVideoFromFrames(fName, fName + "worked_%05d" + generatedFrameExtension)

let files = inputDirectory.GetFiles()
for fNum in 0..files.Length - 1 do
    let f = files[fNum]
    if (ImageProcessor.isImage(f)) then
        processImage(f)
    elif (VideoProcessor.isVideo(f)) then
        processVideo(f)