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
                let roll = rollDice diceCount
                
                printfn $"\n%s{fst players.[i]} rolled %A{diceCount} dice: %A{roll}"
                
                // Get set and remainder score combinations from the roll
                let diceScores:ScoreResults = scoreRoll roll
                let rollTotal = calcTotalScore diceScores

                let hasWon = (snd players.[i] + roundTotal + rollTotal) >= winningScore

                // If the current player has achieved the winning amount (won the game)
                if hasWon then
                    displayGameOver players.[i]
                    turnOver <- true
                    gameOver <- true
                // If they failed to score any points in the current roll
                else if rollTotal = 0 then
                    System.Console.Clear()
                    printfn $"-%s{fst players.[i]} was farkled!-"
                    roundTotal <- 0
                    turnOver <- true
                else
                    printfn $"- Current round total: %d{roundTotal}"

                    // Display the scores for the dice they roll
                    displayScores diceScores diceCount rollTotal
                    
                    // Show the available choices based on the score from the roll
                    let choice = getChoice diceScores diceCount

                    // ---- Act on chosen choice ----
                    match choice with
                    | RollAgain -> 
                        let (newDiceCount, newRoundTotal) = rollAgain diceScores diceCount roundTotal
                        diceCount <- newDiceCount
                        roundTotal <- newRoundTotal
                    | BankPoints ->
                        roundTotal <- roundTotal + rollTotal
                        players <- bankPoints players roundTotal i
                        turnOver <- true
                    | RollAllAgain -> 
                        System.Console.Clear()
                        roundTotal <- roundTotal + rollTotal
                        diceCount <- 6 // All dice can be re-rolled on Hot Dice
    0