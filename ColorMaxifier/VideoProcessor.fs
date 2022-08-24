module VideoProcessor
open System.Diagnostics
open System.IO
open System
open System.Collections.Generic

type VideoProcessor(path: string) = 

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

    member this.videoFromFrames(framerate: int, namePattern: string, resultName: string) = 
        printfn "Creating video from frames w/:\n\tframerate:%d\n\tsaved to:%s" framerate resultName
        let exitCode = startProcess(sprintf $"-r {framerate} -i {namePattern} {resultName}")
        if exitCode <> 0 then failwith "Failed to run ffmpeg"
        