# ProjectOrogeny
Fifth Season "Feral" Survival Game

## WIP Feature List
* **Push** -- Push the earth away from the Orogene
* **Pull** -- Pull the earth towards the Orogene
* **Sess** -- The Orogene's awareness goes into the Earth and toward the focal point. Time is slowed.
* **Sess Aim Mechanic** -- the Orogene can aim at different earthern geometry 
* **Torus** - the ice counter-weight to using Orogeny. Why does land around Orogeny get so cold?

## PUSH
Push is a mechanic that allows the Orogene to pick up earth and fling it. I'm experimenting with a few variations on this. Gifs to come soon:
* Orogene adds a force vector directly to a piece of earth, pushing it forward
* Orogene pops the earth up off of the ground, then applies a forward force

## PULL
Pull is a mechanic that allows the Orogene to pull the earth towards them. 

I love the pull mechanic in Control. How the object comes towards the player then floats in place. Currently Pull is achieved by applying a force vector in the direction of the player, this is crude, I'd like a bit more control. The direction vector is important! It allows one to use Move Position and iterate that position over time. Remember to throw that in the FixedUpdate() since its a Physics call!

## SESS

My notes include phrases like, think about mud with tires running through it. That's the crust, that's where we want the Orogene to focus for accuracy, and time to slow. Think the Predator bow from Assasins Creed Odyssey. Sess can be used to solve puzzles in the enviroment. Mostly movement puzzles aiding the Orogene in crossing dangerous terrain.
