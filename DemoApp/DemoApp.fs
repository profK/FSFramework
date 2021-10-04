module DemoApp

open FSFramework



type DemoApp() =
    interface Application with
        member self.init(args:string[]) =
            printfn $"Command line args %A{args}"
        member self.render() =
            printfn "Render Frame"
        member self.update(deltaMS) =
            printfn $"Update deltaMS %d{deltaMS}"
            true

[<EntryPoint>]
let main(args) =
    
    FSFramework.Start args (new DemoApp())
    0
