module ScoringLibrary

// Player can either score points from rolling a specific set of dice or from certain dice that score points by them selves
type SetTotal = SetTotal of int
type RemainderTotal = RemainderTotal of int

type Dice = Dice of int
type DiceList = Dice list

type ScoreResult =
    | SetCombination of (DiceList * SetTotal)
    | RemainderCombination of (DiceList * RemainderTotal)

type ScoreResults = ScoreResult list


// Get int value of totals
let setTotalToInt = function SetTotal x -> x
let remainderTotalToInt = function RemainderTotal x -> x 
let diceToInt = function Dice x -> x


// Returns the number of times a given dice appears within a DiceList
let countOccurrences (diceToFind:Dice) (roll:DiceList) =
    let mutable count = 0
    for dice in roll do
        if diceToFind = dice then count <- count + 1
    count


// --- Standard Scoring Convention Logic ---
let parseDice dice numOfSets numOfRemainders =
    match dice, numOfSets with
    | 1, _ -> SetTotal (300 * numOfSets), RemainderTotal (100 * numOfRemainders)
    | 5, _ -> SetTotal (500 * numOfSets), RemainderTotal (50 * numOfRemainders)
    | _, _ when numOfSets > 0 ->  SetTotal ((dice * 100) * numOfSets), RemainderTotal 0 // Any dice other than 1 or 5 with at least one set
    | _, _ -> SetTotal 0, RemainderTotal 0


let getSetDice numOfSets (dice:Dice) (setTotal:SetTotal)  =
    let mutable scoreList:ScoreResults = []
    for _ in 1..numOfSets do
        scoreList <- scoreList @ [SetCombination ([dice; dice; dice], setTotal )]
    scoreList


let getRemainderDice numOfRemainders (dice:Dice) (remainderTotal:RemainderTotal)  =
    [RemainderCombination ([for _ in 1 .. numOfRemainders -> dice], remainderTotal )]


// Returns a list of the scoring combinations along with their score
let scoreRoll (currentRoll:DiceList) =
    
    let mutable scoreList:ScoreResults = []
    
    for currentDice in 1..6 do
        
        // Get the number of times the current dice appears within the current roll
        let count = countOccurrences (Dice currentDice) currentRoll
        
        if count > 0 then
            
            // Get number of sets for the current dice within the roll
            let numOfSets = (count - (count % 3)) / 3 
            
            // Get number of times the dice occurs excluding the sets of 3
            let numOfRemainders = count % 3

            // Get the score for the current dice number in the roll
            let mutable score = parseDice currentDice numOfSets numOfRemainders

            // Add each scoring sets of three for the current dice number to the score list (if present)
            if fst score > SetTotal 0 then
                scoreList <- scoreList 
                    @ (getSetDice numOfSets (Dice currentDice) (fst score))
            
            // Add each individually scoring dice for the current dice number to the score list (if present)
            if snd score > RemainderTotal 0 then
                scoreList <- scoreList 
                    @ (getRemainderDice numOfRemainders (Dice currentDice) (snd score))

    scoreList
    