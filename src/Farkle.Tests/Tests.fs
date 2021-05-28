module Tests

open Xunit
open FsCheck
open FsCheck.Xunit

open ScoringLibrary
open GameLibrary

let possibleSets = [
    ([Dice 1; Dice 1; Dice 1], 300);
    ([Dice 2; Dice 2; Dice 2], 200);
    ([Dice 3; Dice 3; Dice 3], 300);
    ([Dice 4; Dice 4; Dice 4], 400);
    ([Dice 5; Dice 5; Dice 5], 500);
    ([Dice 6; Dice 6; Dice 6], 600);
]

let possibleRemainders = [
    ([Dice 1], 100);
    ([Dice 5], 50);
    ([Dice 1; Dice 1], 200);
    ([Dice 5; Dice 5], 100);
]

[<Fact>]
let ``My test`` () = 
    for set in possibleSets do
        let expected = SetCombination (fst set, SetTotal (snd set))
        let actual = (scoreRoll (fst set)).[0]
        printfn "Expected: %A, actual: %A" expected actual
        Assert.Equal(expected, actual)

``My test`` ()

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