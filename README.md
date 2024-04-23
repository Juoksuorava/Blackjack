Simple console-based blackjack game in F#. This game was created for UTU course DTEK0071 Programming Paradigms in Practice.

This version of blackjack involves one player (you) playing against the dealer (computer). The goal is to draw cards one by one and get as close to 21 without going over. 

The documentation of this program is this README file. The source code also has some comments I would not normally make in order to better explain what the code does and explain some concepts and features of F#.
## Gameplay
### Actions
Player can on their turn, which is always first, either (H)it or (S)tand. Hitting means to draw another card from the deck and standing means passing the turn over to the dealer. The player can also Q)uit the game, which just closes the program. Typing in anything else but **H | S | Q** will just loop the player in the current state of the game forever.  Finally, after a round is over, the player can type in (Y)es in order to start another round.

### UI
Here's an example of what one round of this blackjack might look like on your terminal

PLAYER: (10)
Eight of Clubs
Two of Clubs
DEALER: (10)
Ten of Spades
******
(H)it | (S)tand | (Q)uit: h (Here is where the player has inputted *h*)

Drawn card:
Eight of Diamonds
PLAYER: (18)
Eight of Diamonds
Eight of Clubs
Two of Clubs
DEALER: (10)
Ten of Spades
******
(H)it | (S)tand | (Q)uit: s (Here is where the player has inputted *s*)

Dealer goes now!
PLAYER: (18)
Eight of Diamonds
Eight of Clubs
Two of Clubs
DEALER: (15)
Ten of Spades
Five of Hearts
Dealer hits! (22)
Seven of Hearts
Ten of Spades
Five of Hearts
Dealer busted.
You win!

------

## Functional programming paradigm
F# is a multi-paradigm language and this program implements a lot of functional programming concepts, such as immutability, first-class and higher-order functions and pattern matching. However, this code is not purely functional due to few things.

1. Mutable state
	```fsharp
	type Deck() =
	    member val Cards: Card list = [] with get, set
    ```
    - This is an example of mutable state in the code. In this example the *Cards* member is mutable.
2. Side effects
	```fsharp
	printf "Play again (y)es/no? "
	```
	- This is an example of side effects in the code. Calling the *printf* function interacts with the outside world (the console in this case). Functions should not have side effects in purely functional programming.
3. Non-referential transparency
	```fsharp
	printf "Play again (y)es/no? "
	```
	- This is an example of non-referential transparency. Calling the *blackjack* function with the same input does not always give the same output, since the function involves drawing cards from a deck, which changes the state of the deck.
To make this program follow more a functional programming paradigm some changes could be implemented.
1. Avoid mutable state
	- Data should be immutable in functional programming. Instead of mutating the *Cards* member of the *Deck* type the could return a new *Deck* instance with the updated cards every time.
2. Use pure functions
	- Pure functions do not have side effects and always produce the same output given the same input. The *blackjack* function could be made pure by passing the current *Deck* as an argument and finally returning the updated *Deck* to keep the game state true.
3. Avoid side effects
	- Instead of having the game state printed to the console inside the *blackjack* function, the code could return a data structure representing the game state and have the caller of the function print the state.
However, in conclusion, while the code of this program is not purely function, it represents quite a common reality of programming where a mix of functional and imperative (or object-oriented) concepts are fused together to better the program and the programming experience. This is why multi-paradigm languages like F# are a good choice to learn.

## What if?
For the sake of analyzing whether or not F# supported creating this short game with its multi-paradigm features I reminiscence the time I created a console-based blackjack game in Java for another course some years ago. The code is lost but some memories still exist.

F# didn't hinder the approach to this program, since it very well allowed the modeling of the real world components that appear in blackjack in code. Creating the types was very much like creating classes in Java for objects that represent the different components of the game. Maybe the boilerplate of Java actually hindered the coding more, since for this exercise writing the short types into the main source file of the code felt very natural.

Active pattern matching allowed for very easy handling of the user inputs to progress with the game flow. I remember trying to create user input handling in Java was very hard, since I had to think of all the possible valid and invalid states. 

F# will most likely me a language I will try out more in the future. Hopefully, I get to apply more of a purely functional approach to it as well.