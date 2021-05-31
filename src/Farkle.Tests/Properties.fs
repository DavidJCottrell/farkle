module GameProperties

open ScoringLibrary
open GameLibrary
open System

// Properties

// --- DiceList properties ---

// let myFunc num =
//     num + 1

// let myFuncThatUsesMyFunc num =
//     myFunc num = num + 1

// let testRollDice numOfDice =
//     snd (rollDice numOfDice)

// A given DiceList should never exceed 6 dice
// let lessThan6DiceProperty (diceList:DiceList) =
//     List.length diceList <= 6

// A given DiceList should always have more than 0 dice
// let moreThan0DiceProperty (diceList:DiceList) =
//     List.length diceList > 0

// --- ScoreResults ---
// [(DiceList * SetTotal | RemainderTotal), ...]

// A given ScoreResults should never have more than 2 SetCombinations
// let noMoreThan2Sets numOfDice =
//     let roll = rollDice numOfDice
//     let scoreResults = scoreRoll roll
//     let mutable setCount = 0
//     for score in scoreResults do
//         match score with
//         | SetCombination _ -> setCount <- setCount + 1
//         | _ -> ()
//     setCount <= 2

// // A given ScoreResults should never have more than 4 RemainderCombinations
// let noMoreThan4Remainders (scoreResults:ScoreResults) =
//     let mutable remainderCount = 0
//     for score in scoreResults do
//         match score with
//         | RemainderCombination _ -> remainderCount <- remainderCount + 1
//         | _ -> ()
//     remainderCount <= 4


// --- scoreRoll ---

// When scoreRoll is called with 3 or 6 of any of the same number dice, it should return 1 SetCombination
// let hasSetCombinations dice numOfSets =
    
//     // [Dice 3; Dice 3; Dice 3; Dice 3; Dice 3; Dice 3;]
//     let sets = List.init (numOfSets * 3) (fun _ -> Dice dice)

//     let actualSets = scoreRoll sets
    

//     for actualSet in actualSets do
//         let total = if dice = 1 then 300 else (dice * 100)
//         let expected = SetCombination ([Dice dice; Dice dice; Dice dice], SetTotal total)

// When scoreRoll is called with 3 of any of the same number dice, it should return 1 SetCombination
// let oneSetCombinationFrom3IdenticalDice (dice:Dice) = 
//     let set = [dice; dice; dice]
//     let mutable total = 0
//     if set.[0] = Dice 1 then total <- 300 else total <- 100 * (diceToInt set.[0])
//     (scoreRoll set) = [SetCombination (set, SetTotal total)]

// A SetCombination should be present if 3 of any of the same number dice present

    

// A RemainderTotal should be present if 1 or 2 of Dice 1 or Dice 5


// A given ScoreResult should contain the appropriate Total and Dice within that score
