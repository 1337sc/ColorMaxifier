open ImageProcessor
open System.Drawing
open System.IO
open System
open System.Text.RegularExpressions
open System.Collections.Generic
open VideoProcessor

let inputFolder = "./input"
let outputFolder = "./output"

let generatedFrameExtension = ".png"
let generatedVideoExtension = ".mp4"
let generatedAudioExtension = ".aac"

let videoProcessor = new VideoProcessor(Path.GetFullPath("ffmpeg path"))

let inputDirectory = new DirectoryInfo(Path.GetFullPath(inputFolder))
let outputDirectory = new DirectoryInfo(Path.GetFullPath(outputFolder))

if Directory.Exists(Path.GetFullPath(outputFolder)) |> not then
    Directory.CreateDirectory(Path.GetFullPath(outputFolder)) |> ignore

let ensureDeleted(path: string) = 
    if (File.Exists(path)) then
        File.Delete(path)
    if (Directory.Exists(path)) then
        Directory.Delete(path)

let createVideoFromFrames(fName: string, framesPattern: string) = 
    let outputFile = Path.GetFullPath(Path.Combine(outputFolder, fName + generatedVideoExtension))
    ensureDeleted(outputFile)

    videoProcessor.videoFromFrames(30, Path.GetFullPath(Path.Combine(outputFolder, framesPattern)), outputFile)

    outputFile

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

    createVideoFromFrames(fName, fName + "%05d" + generatedFrameExtension) |> ignore

let processVideo(f: FileInfo) = 
    let fName = f.Name.Replace(f.Extension, String.Empty)
    let audioPath = Path.GetFullPath(Path.Combine(outputFolder, fName + "_audio" + generatedAudioExtension))
    ensureDeleted(audioPath)

    videoProcessor.extractAudioFromVideo(f.FullName, audioPath)
    videoProcessor.framesFromVideo(f.FullName, Path.GetFullPath(Path.Combine(outputFolder, fName + "%05d" + generatedFrameExtension)))
    let frames = outputDirectory.GetFiles(fName + "*" + generatedFrameExtension)
    let mutable meanTime = 0
    for c in 0..frames.Length - 1 do
        let frame = frames[c]
        task {
            let timeStart = DateTime.UtcNow
            let imageProcessor = new ImageProcessor(new Bitmap(Bitmap.FromFile(frame.FullName)), 
                56, 
                142,
                67,
                153,
                78,
                164)
            let newFilePath = Path.GetFullPath(Path.Combine(outputFolder, 
                fName + "worked_" + c.ToString().PadLeft(5, '0') + generatedFrameExtension))
            imageProcessor.saveAddedToEachColor(newFilePath)
            let timePassed = DateTime.UtcNow - timeStart
            meanTime <- meanTime + timePassed.Milliseconds
        }
        |> Async.AwaitTask
        |> ignore
    
    printfn "Mean time processing frames: %d ms" (meanTime / frames.Length)

    let resultVideoPath = fName + "worked_%05d" + generatedFrameExtension
    let result = createVideoFromFrames(fName, resultVideoPath)
    let soundedPath = Path.GetFullPath(Path.Combine(outputFolder, fName + "_sounded" + generatedVideoExtension))
    ensureDeleted(soundedPath)
    videoProcessor.copyAudioIntoVideo(result, audioPath, soundedPath)

let files = inputDirectory.GetFiles()
for fNum in 0..files.Length - 1 do
    let f = files[fNum]
    if (ImageProcessor.isImage(f)) then
        processImage(f)
    elif (VideoProcessor.isVideo(f)) then
        processVideo(f)