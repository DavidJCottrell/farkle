module App

open GameLibrary
open ScoringLibrary

[<EntryPoint>]
let main _ =
    
    let mutable players:Players = []
    players <- getPlayers
    System.Console.Clear() // Clear player entry dialog

    let mutable gameOver = false
    let winningScore = 10000

    // Loop while game isn't over
    while not gameOver do

        // Give each each player their turn
        for i in 0 .. (List.length players) - 1 do            

            // Display scoredboard if game hasn't already ended
            if not gameOver then displayScoreBoard players

            let mutable diceCount = 6 // Number of dice thrown within a given round
            let mutable roundTotal = 0
            let mutable turnOver = false

            // While it's the current players turn and the game isn't over
            while not turnOver && not gameOver do

                printfn $"- Current player: %s{fst players.[i]} -"

                // Roll some number of random dice
                let rollResult = rollDice diceCount
                
                // If an invalid number of dice occurs
                if snd rollResult = false then failwith "Invalid number of dice thrown."
                let roll = fst rollResult // Get the valid dice list
                
                printfn $"\n%s{fst players.[i]} rolled %A{diceCount} dice: %A{roll}"
                
                // Get set and remainder score combinations from the roll
                let scoreResults:ScoreResults = scoreRoll roll
                let rollTotal = calcTotalScore scoreResults

                // If the current player has achieved the winning amount (won the game)
                let hasWon = (snd players.[i] + roundTotal + rollTotal) >= winningScore

                if hasWon then
                    displayGameOver players.[i]
                    turnOver <- true
                    gameOver <- true
                else
                    
                    // Show the available choices based on the score from the roll
                    let choice = getChoice scoreResults rollTotal roundTotal diceCount players.[i]

                    // ---- Act on chosen choice (or fail) ----
                    match choice with
                    | RollAgain -> 
                        let (newDiceCount, newRoundTotal) = rollAgain scoreResults diceCount roundTotal
                        diceCount <- newDiceCount
                        roundTotal <- newRoundTotal
                    | BankPoints ->
                        roundTotal <- roundTotal + rollTotal
                        players <- bankPoints players roundTotal i
                        turnOver <- true
                    | RollAllAgain -> 
                        roundTotal <- roundTotal + rollTotal
                        diceCount <- 6 // All dice can be re-rolled on Hot Dice
                    | Fail ->
                        printfn $"-%s{fst players.[i]} was farkled!-"
                        roundTotal <- 0
                        turnOver <- true
    0