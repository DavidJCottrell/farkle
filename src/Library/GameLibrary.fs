module GameLibrary

open ScoringLibrary
open System

type Name = String
type Score = int
type Player = (Name * Score)
type Players = Player list

type Choice =
    | BankPoints
    | RollAgain
    | RollAllAgain
    | Fail

// Returns a given amount of random numbers (Dice)
let rollDice (numOfDice:int) : DiceList = 
    let rand = Random()
    List.init numOfDice (fun _ -> Dice (rand.Next (1, 7)))


// Ask user for their choice to Roll Again, Bank Points or roll all dice again if Hot Dice occurs
let getChoice (scores:ScoreResults) (rollTotal:int) (roundTotal:int) (diceCount:int) (player:Player) =
    let mutable numOfScoringDice = 0
    // Get the number of dice that formed the score combinations
    for score in scores do
        numOfScoringDice <- numOfScoringDice + 
            match score with
            | SetCombination (dice, _) -> List.length dice
            | RemainderCombination (dice, _) -> List.length dice
    
    let mutable choice = Fail // Default placeholder value
    
    // User has no choices if they fail to score
    if not (rollTotal = 0) then
        
        // Display scoring dice, the roll total and the players round total
        printfn $"\n%d{numOfScoringDice}/%d{diceCount} dice scored. Scoring dice:"
        let mutable count = 1
        for score in scores do
            match score with
            | RemainderCombination (x, y) -> printfn $"%d{count}) %A{x}, with a total of %d{remainderTotalToInt y}"
            | SetCombination (x, y) -> printfn $"%d{count}) %A{x}, with a total of %d{setTotalToInt y}"
            count <- count + 1
        printfn $"\n- Current roll total: %d{rollTotal}"
        printfn $"- Current round total: %d{roundTotal}"
        printfn $"- Your total score (%d{snd player}) + round total + roll total = %d{roundTotal + rollTotal + snd player}\n"

        // --- Display options for if the user scored AND get their choice ---
        let mutable validChoice = false 
        while not validChoice do
            
            let validateChoice (success, choiceInt) (choice1:Choice) (choice2:Choice) =
                match success, choiceInt with
                | true, 1 -> 
                    choice <- choice1
                    validChoice <- true
                | true, 2 -> 
                    choice <- choice2
                    validChoice <- true
                | _, _ -> printfn "Invalid input, please try again."

            // --- Hot dice options ---
            if numOfScoringDice = diceCount then
                printfn "Hot Dice!"
                printfn "Would you like to bank your points (1) or roll all 6 dice again? (2)"
                printf "> "
                validateChoice (System.Int32.TryParse(System.Console.ReadLine())) BankPoints RollAllAgain
            
            // --- Normal scoring options ---
            else
                printfn "Would you like to bank your points (1) or roll again? (2)"
                printf "> "
                validateChoice (System.Int32.TryParse(System.Console.ReadLine())) BankPoints RollAgain
    choice


// Allows the user to choose which dice they want to keep if they choose to re-roll
let chooseRollAgainScores (scoreList:ScoreResult list) =
    let mutable parsedChoices = []
    printfn "Enter the dice you would like to keep from the above list (separate choices with spaces)"

    let mutable valid = false
    while not valid do

        printf "> "
        let choices = System.Console.ReadLine().Split [|' '|] |> Array.toList // Unvalidated user input
        
        // Choice validator function
        let validateScoreChoice (value:String) =
            let (success, num) = System.Int32.TryParse(value)
            if success && num > 0 && num <= (List.length scoreList) && List.length choices <= List.length scoreList then num
            else -1

        // Validate each individual choice
        parsedChoices <- List.map (fun choice -> validateScoreChoice choice) choices
        
        // If any of the choices were invalid
        if List.exists ((=) -1) parsedChoices then printfn "Invalid input, please try again."
        else valid <- true

    let mutable chosenScores = []
    for choice in parsedChoices do
        // Get the chosen scores from the list of choices
        chosenScores <- chosenScores @ [scoreList.[choice - 1]]
    
    System.Console.Clear()
    chosenScores


// Returns the total score for a given roll
let calcTotalScore (scoreList:ScoreResult list) =
    let mutable RollTotal = 0       
    for score in scoreList do
        RollTotal <- RollTotal 
            + match score with
              | SetCombination (_, setTotal) -> setTotalToInt setTotal
              | RemainderCombination (_, remainderTotal) -> remainderTotalToInt remainderTotal
    RollTotal


// Displays the scoreboard
let displayScoreBoard (players:Players) =
    printfn "\nScore board"
    printfn "---------------------"
    for player in players do
        printfn "- %s has %d points" (fst player) (snd player)
    printfn "---------------------\n"


// Displays the winning player once a game is won
let displayGameOver (player:Player) =
    System.Console.Clear()
    printfn "\n\n"
    printfn "--------------------------------"
    printfn " %s wins!" (fst player)
    printfn "--------------------------------"
    printfn "\n\n"
  
// Asks user to input the names of at least two players
let getPlayers =
    let mutable keepAdding = true
    let mutable players = []
    printfn "- Add players -"
    printfn "Enter player's name or type 'stop' to stop."
    while keepAdding do
        printf "> "
        let name = System.Console.ReadLine()
        if name = "stop" then 
            if List.length players >= 2 then
                keepAdding <- false
            else
                printfn "Please add at least two players."
        else
            if not (name = "") then players <- players @ [(name, 0)]
    players

// (newDiceCount, newRoundTotal)
let rollAgain (scoreResults:ScoreResults) diceCount roundTotal =
    let mutable newDiceCount = diceCount
    let mutable newRoundTotal = roundTotal
    // Ask user to choose the dice they want to keep
    let chosenScores = chooseRollAgainScores scoreResults

    // Show the dice the user decided to keep
    printfn "You chose to keep: "
    for chosenScore in chosenScores do
        match chosenScore with
        | SetCombination (dice, _) -> printfn $"-> %A{dice}"
        | RemainderCombination (dice, _) -> printfn $"-> %A{dice}"

    // Subtract the kept dice from the amount of dice to re-roll and their scores to the round total
    for chosenScore in chosenScores do
        match chosenScore with
        | SetCombination (dice, total) ->
            newDiceCount <- newDiceCount - List.length dice
            newRoundTotal <- newRoundTotal + setTotalToInt total
        | RemainderCombination (dice, total) ->
            newDiceCount <- newDiceCount - List.length dice
            newRoundTotal <- newRoundTotal + remainderTotalToInt total
    (newDiceCount, newRoundTotal)

// Updates the current player's score within the list of players with their new total score
let bankPoints (players:Players) (roundTotal:int) (playerIndex:int) : Players =
    System.Console.Clear()
    printfn $"-%s{fst players.[playerIndex]} banked their points-"
    let updatePlayer index addFunc players = 
        let mutable i = -1
        List.map (fun (name, score) -> 
                i <- i + 1
                if i = index then name, addFunc score 
                else name, score
        ) players
    updatePlayer playerIndex (fun score -> score + roundTotal) players