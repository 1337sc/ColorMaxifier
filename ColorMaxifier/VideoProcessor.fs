module VideoProcessor
open System.Diagnostics
open System.IO
open System
open System.Collections.Generic

type VideoProcessor(path: string) = 

    static let videoExtensions = [".AVI"; ".MPEG"; ".MP4"; ".FLV"]

    let startProcess(command: string) = 
        use proc = new Process()
        let pi = ProcessStartInfo path
        let dir = Path.GetDirectoryName path
 
        pi.CreateNoWindow <- true
        pi.ErrorDialog <- false
        pi.UseShellExecute <- false
        pi.Arguments <- command
        pi.WorkingDirectory <- dir
     
        proc.StartInfo <- pi

        printfn "Starting %s w\:%s" dir command
 
        if not (proc.Start()) then 1
        else
            proc.WaitForExit()
            proc.ExitCode

    static member isVideo(file: FileInfo) = 
        List.contains (file.Extension.ToUpper()) videoExtensions

    member this.videoFromFrames(framerate: int, namePattern: string, resultName: string) = 
        printfn "Creating video from frames w/:\n\tframerate:%d\n\tsaved to:%s" framerate resultName
        let exitCode = startProcess(sprintf $"-r {framerate} -i {namePattern} {resultName}")
        if exitCode <> 0 then failwith "Failed to run ffmpeg"

    member this.framesFromVideo(fromPath: string, resultNamesPattern: string) = 
        printfn "Creating frames from video w/:\n\tfrom:%s\n\tsaved to:%s" fromPath resultNamesPattern
        let exitCode = startProcess(sprintf $"-i {fromPath} {resultNamesPattern}")
        if exitCode <> 0 then failwith "Failed to run ffmpeg"

    member this.extractAudioFromVideo(fromPath: string, resultName: string) = 
        printfn "Extracting audio from %s, saving to %s" fromPath resultName
        let exitCode = startProcess(sprintf $"-i {fromPath} -map 0:a -acodec copy {resultName}")
        if exitCode <> 0 then failwith "Failed to run ffmpeg"

    member this.copyAudioIntoVideo(videoPath: string, audioPath: string, resultPath: string) = 
        printfn "Merging audio from %s into %s" audioPath videoPath
        let exitCode = startProcess(sprintf $"-i {videoPath} -i {audioPath} -c copy {resultPath}")
        if exitCode <> 0 then failwith "Failed to run ffmpeg"
        