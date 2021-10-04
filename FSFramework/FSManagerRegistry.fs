module ManagerRegistry 
open System
open System.Reflection
open FSharp.Collections

[<Flags>]
type supportedSystems =
    Windows=1 | Mac=2 | Linux=4 | Wasm=8

// marker Attribute
type Manager(prettyName:string, systems:supportedSystems) =
    inherit Attribute()
    member self.SupportedSystems = systems
    member self.Name = prettyName;
    
let mutable managers : Map<string, obj list> = Map.empty
let addManager (managerType:Type) : unit =
    let interfaces = managerType.GetInterfaces()
    let instance = Activator.CreateInstance(managerType);
    managers <-
            interfaces
            |> Array.fold
                (fun mgrs i -> mgrs |> Map.change i.FullName (Option.defaultValue [] >> fun ml -> Some(instance :: ml)))
                managers                                                                  
let scanAssembly(assembly:Assembly) =
    assembly.GetTypes() |>Seq.filter (
        fun atype -> not ((atype.GetCustomAttribute typedefof<Manager>)|> isNull)) |>
        Seq.iter(addManager)                                                                         
let getManager<'I>(): 'I option =
       let typeObj = typeof<'I>
       if typeObj.IsInterface then
           managers
           |> Map.tryFind typeObj.FullName
           |> Option.map (function
               | m::rest -> m :?> 'I
               | [] -> failwith $"Invalid state, manager list for {typeObj.FullName} was empty")
       else
           invalidArg typeObj.FullName "Manager types must be interfaces"
let rec castList<'a> (myList: obj list) =          
    match myList with
    | head::tail -> 
        match head with 
        | :? 'a as a -> a::(castList tail) 
        | _ -> castList tail
    | [] -> []              
let getAllManager<'I>(): 'I list option =
       let typeObj = typeof<'I>
       if typeObj.IsInterface then
           managers |> Map.tryFind typeObj.FullName |> Option.map (fun mgrls -> mgrls |> castList<'I>)
       else
           invalidArg typeObj.FullName "Manager types must be interfaces"     
       
let getAllManagers<'I>(): 'I list option =
       let typeObj = typeof<'I>
       if typeObj.IsInterface then
           managers
           |> Map.tryFind typeObj.FullName
           |> Option.map castList<'I>
       else
           invalidArg typeObj.FullName "Manager types must be interfaces"
    