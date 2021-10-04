module FSFramework

open System


type Application =
    abstract init: string[] -> unit 
    abstract update: int -> bool  
    abstract render: unit -> unit
 
    
let Start (args:string[]) (app:Application) : unit =
    app.init (args)
    let mutable lastTime = DateTime.Now.Millisecond
    let mutable thisTime = DateTime.Now.Millisecond
    while app.update (thisTime - lastTime) do
        app.render() 
        lastTime <- thisTime
        thisTime <- DateTime.Now.Millisecond
        
    

            
            

    