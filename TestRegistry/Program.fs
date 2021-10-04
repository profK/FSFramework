// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp
module TestRegistry

open System
open System.Reflection
open ManagerRegistry

// Define a function to construct a message to print
type TestManagerInterface =
    abstract printme: unit -> unit
    
type UnusedInterface =
    abstract printme: unit -> unit    
     
[<Manager("Test Manager",supportedSystems.Linux|||supportedSystems.Windows)>] 
type mymanager() =
    interface TestManagerInterface with
        member this.printme() =
            printfn "Hello Manager!" |> ignore
            
[<EntryPoint>]            
ManagerRegistry.scanAssembly(Assembly.GetCallingAssembly())
let mgr = ManagerRegistry.getManager<TestManagerInterface>()
match mgr with
| Some(m)-> m.printme() |> ignore //expected result
| None -> printfn "Did not find manager" |> ignore

let mgr2 = ManagerRegistry.getManager<UnusedInterface>()
match mgr2 with
| Some(m)-> m.printme() |> ignore
| None -> printfn "Did not find manager" |> ignore // expected result