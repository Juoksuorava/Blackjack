open System

// In order to build a complete deck of cards to draw from we need to represent all the four suits found in typical playing cards
// For evaluation of a hand the suit of a card is not important
type Suit = 
    | Hearts
    | Diamonds
    | Clubs
    | Spades
    static member list = [Hearts;Diamonds;Clubs;Spades]

// In order to build a complete deck of cards to draw from we need to represent all the ranks found in typical playing cards
// Ranks are also important for the evaluation of a hand
type Rank =
    | Ace
    | Two
    | Three
    | Four
    | Five
    | Six
    | Seven
    | Eight
    | Nine
    | Ten
    | Jack
    | Queen
    | King
    static member list = [Ace;Two;Three;Four;Five;Six;Seven;Eight;Nine;Ten;Jack;Queen;King]
    // Return the correct value for each rank
    static member value (rank : Rank) =
        match rank with
            | Ace -> 11
            | Two -> 2
            | Three -> 3
            | Four -> 4
            | Five -> 5
            | Six -> 6
            | Seven -> 7
            | Eight -> 8
            | Nine -> 9
            | Ten -> 10
            | Jack -> 10
            | Queen -> 10
            | King -> 10

// TType for complete playing card
type Card =
    {
        Suit: Suit;
        Rank: Rank
    }
    // Value of card is the value of its rank
    member this.value() =
        (Rank.value(this.Rank))
    // Overloads the addition operator to add the values of two card types together
    static member (+) (cardA: Card, cardB: Card) =
        (cardA.value() + cardB.value())
            // Overloads the addition operator to add the values a card and an integer together
    static member (+) (card: Card, number: int) =
        (card.value() + number)

// String representation of a card to print inside a console
let cardString(card: Card) =
     "\n" + card.Rank.ToString() + " of " + card.Suit.ToString()

// Type for complete playing deck
type Deck() =
    member val Cards: Card list = [] with get, set
    member this.remaining() =
        this.Cards.Length
    member this.shuffle() =
        let random = Random()
        this.Cards <- this.Cards |> List.sortBy (fun _ -> random.Next())
    // Draw a card from the deck
    // If deck is empty before drawing, refills and shuffles it
    member this.draw() =
        if this.Cards.Length < 1 then
            this.Cards <- [
                for suit in Suit.list do
                    for rank in Rank.list do
                        yield { Suit = suit ; Rank = rank }]
            this.shuffle()
        let card = this.Cards.Head
        this.Cards <- this.Cards.Tail
        card

// Helper function to match a string prefix for player actions
// Example of active patterns in F#
let (|Prefix|_|) (p: string) (s: string) =
    if s.StartsWith(p) then
        // If condition met then return the part of s that comes after p
        Some(s.Substring(p.Length))
    else
        None

// Type to represent possible player actions
type Action =
    | Hit
    | Stand
    | Quit
    | Default
    static member parse(inputString: string) =
        if inputString.Length < 1 then
            Default
        else
            match inputString.ToLower() with
                | Prefix "h" _ -> Hit
                | Prefix "s" _ -> Stand
                | Prefix "q" _ -> Quit
                | _ -> Default

// Main function of the game
let blackjack(deck: Deck) =
    let player: Card list = [deck.draw(); deck.draw()]
    let dealer: Card list = [deck.draw(); deck.draw()]

    let evaluateHand (hand: Card list) =
        let mutable highAces = 0
        let mutable sum = 0
        // For-loop to iterate through the whole hand
        for i in 0..hand.Length - 1 do
            sum <- hand[i] + sum
            if hand[i].Rank = Ace then
                highAces <- highAces + 1
            while highAces > 0 && sum > 21 do
                sum <- sum - 10
                highAces <- highAces - 1
        (sum)

    let handString(hand: Card list) =
        ("(" + evaluateHand(hand).ToString() + ")").PadLeft(4) + " " + (String.concat "" (List.map (fun elem -> cardString(elem).PadRight(8)) hand))

    let displayHandsPlayerRound(player: Card list, dealer: Card list) =
        printf "PLAYER: %s\n" (handString player)
        // List[n..m] returns a copy of a list with only values n through m
        // e.g. below we only give the handString only the first card in the dealers hand
        printf "DEALER: %s\n******\n" (handString dealer[0..0])
    
    let displayHandsDealerRound(player: string, dealer: Card list) =
        printf "PLAYER: %s\n" player
        printf "DEALER: %s\n" (handString dealer)

    // Rec defines a recursive function
    // Also line bloew uses a complex, record type with named fields, semicolons act as separators in F#
    let rec dealerLoop(player: {|value: int; str: string |}, dealer: Card list, deck: Deck) =
        let value = evaluateHand dealer
        
        let hit() = 
            let drawnCard = deck.draw()
            // A :: B creates a new list with A as the 1st item of the list if applicable
            // e.g. Card type :: Card list
            // or Card type :: Card type as below for our handString
            printf "Dealer hits! %s\n" (handString(drawnCard :: dealer))
            dealerLoop(player, (drawnCard :: dealer), deck)

        // Match..with.. is a supercharged switch/case statements
        match value with
        | _ when value > 21 -> (
            printf "Dealer busted.\nYou win!\n")
        | _ when value > player.value -> (
            printf "Dealer wins.")
        | _ when value < player.value -> (
            hit())
        // _ Underscore matches anything, wihtout condition this becomes the default "case"
        | _ -> (
            if value < 15 then
                hit()
            else
                if value = 21 then
                    printf "Dealer has blackjack. \n"
                printf "Dealer holds.\n It's a tie. \n"
        )

    let rec playerLoop(player: Card list, dealer: Card list, deck: Deck) =
        let value = evaluateHand player

        match value with
        | t when t > 21 -> (
            displayHandsDealerRound(handString player, dealer)
            printf "You bust. \n")
        | 21 -> (
            displayHandsPlayerRound(player, dealer)
            printf "Blackjack! \n"
            printf "\n Dealer's turn.\n"
            dealerLoop({|
                value = 21;
                str = handString player;
                |}, dealer, deck))
         | t when t < 21 -> (
            displayHandsPlayerRound (player, dealer)
            printf "(H)it | (S)tand | (Q)uit: "
            let userInput = Console.ReadLine().ToLower()
            match (Action.parse(userInput)) with
                | Hit -> (
                    let drawnCard = deck.draw()
                    printf "\nDrawn card: %s\n" (cardString(drawnCard))
                    playerLoop(drawnCard :: player,dealer,deck))
                | Stand -> (
                    printf ("\nDealer goes now!\n")
                    displayHandsDealerRound(handString player, dealer)
                    dealerLoop ({|
                        value = evaluateHand player;
                        str = handString(player);
                    |}, dealer, deck))
                | Quit -> ()
                | Default -> playerLoop(player,dealer,deck))

    playerLoop(player,dealer,deck)
    printf "\n------\n"

// Entrypoint of the program
[<EntryPoint>]
let main _ = (
    let deck = Deck()
    let gameState = true
    while gameState do
        blackjack deck
        printf "Play again (y)es/no? "
        let userInput = Console.ReadLine().ToLower()
        match userInput.ToLower() with
        | Prefix "y" _ -> blackjack deck
        | _ -> ()
    
    printfn "Thanks for playing!"

    0
)