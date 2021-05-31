module Tests

open Xunit
open FsCheck
open FsCheck.Xunit

open ScoringLibrary
open GameLibrary

let expectedPossibleSets:ScoreResults = [
    SetCombination ([Dice 1; Dice 1; Dice 1], SetTotal 300);
    SetCombination ([Dice 2; Dice 2; Dice 2], SetTotal 200);
    SetCombination ([Dice 3; Dice 3; Dice 3], SetTotal 300);
    SetCombination ([Dice 4; Dice 4; Dice 4], SetTotal 400);
    SetCombination ([Dice 5; Dice 5; Dice 5], SetTotal 500);
    SetCombination ([Dice 6; Dice 6; Dice 6], SetTotal 600);
]

let expectedPossibleRemainders:ScoreResults = [
    RemainderCombination ([Dice 1], RemainderTotal 100);
    RemainderCombination ([Dice 5], RemainderTotal 50);
    RemainderCombination ([Dice 1; Dice 1], RemainderTotal 200);
    RemainderCombination ([Dice 5; Dice 5], RemainderTotal 100);
]

// [<Fact>]
// let ``A DiceList should never have more than (n - (n % 3)) / 3 sets`` (roll:DiceList) = 
//     let len = List.length roll
//     (len - (len % 3)) / 3


[<Property>]
let ``A DiceList should never have more than (n - (n % 3)) / 3 sets`` (roll:DiceList) = 
    let scoreResults = scoreRoll roll
    let mutable numOfSets = 0
    for score in scoreResults do
        match score with
        | SetCombination _ -> numOfSets <- numOfSets + 1
        | _ -> numOfSets <- numOfSets + 1

    let len = List.length roll
    (len - (len % 3)) / 3

[<Fact>]
let ``test that the maximum number of sets for a given DiceList are never exceeded`` () = 
    let property num =
        //roll some number of dice and check against property
        rollDice num |> ``A DiceList should never have more than (n - (n % 3)) / 3 sets``
    
    Check.QuickThrowOnFailure property




// Check.Verbose ``My test``

// [<Property>]
// let ``three identical dice return a SetCombination for that dice`` diceNum = 

    // let dice = [Dice diceNum; Dice diceNum; Dice diceNum]
    // let score = if diceNum = 1 then 300 else diceNum * 100
    // (diceNum > 0 && diceNum < 7) ==> ((scoreRoll dice).[0] = SetCombination (dice, SetTotal score))

// Check.Verbose ``three identical dice return a SetCombination for that dice``

// let chooseFromList list = 
//     gen { let! i = Gen.choose (0, List.length list-1) 
//         return List.item i list } |> Gen.sample 0 10

// [<Property>]
// let ``a given roll should`` diceNum = 

//     let set = (chooseFromList possibleSets).[0]
    
//     printfn "%A" set
    
//     //let setDice = (fst set)

    
//     true

// Check.Verbose ``a given roll should``
