# Flight app

That's a bit of a fun app made in the winter. The aim was to create a quite realistic 3d flight simulator (godot engine). The plane data would be adjusted (affecting sizes of the elements, amount of engines, engines power etc).
While the results didn't provide realistic control, the plane was flyable and some conclusions where drawn.

# How it was meant to work - flying

After making quite a lot of reasearch about aviatics (really substantial amount of project time spent on it) I've managed to figure out how to make plane flyable from the phsyics point of view. This is important to note that I'd started with simplified equations and then I wad gradually adding more factors (well, and corrected some initial equations - not all of them were correct :D).

I assumed that with those equations I'd be able to develop a quite realistic simullator - I reproduced aerodynamic-significant parts (like wings, ailerons, slats, stabilizers etc.) and wrote keyboard-based control to them.
## Well, the results...
While some functionality actually succeded (predominantly the basic contorl of yaw-pitch-roll angles was quite successful), the overall controll and flight parameters (I'll discuss it in the next point) were either far from great or terrible.
The plane was fully flyable (featuring things like long turns, climbs and descends although landing was proven very challanging so it was rather crashing than properly rolling on the ground), but speed (both vertical and horizontal) was unrealistic, the lift (absolutely crucial parameter) was varying much too fast (the basic idea of falling when the lift was smaller than the actual weight of the plane was working properly however) and the controls where quite hard to manage.

# Flight data analysis
Much time was also spent on making controls imitanting those in real planes (like artificial horizon, speed meters, fuel meters etc, qutie a lot in total btw). Actually this part of the project was actually working nice and the retreived data proven that many parameters were unrealistic. Additionaly I included detailed text-based data (to monitor every single part of the plane, to the point that most of the screen was actually covered with text and controls rather than with the terrain 3d view) in a bit of a debug fashion which was a priceless help in finding problems and bugs.
# Conclusions and comparisionn with free simulators
Eventually I compared already existing free simulators with my own. While they were (obviously ;( ) leaps and bounds above my software I was quite suprised that I've managed to catch an interesting detail I hadn't been aware of previously.
In my app I've noticed that heavy change of the roll angle triggered change of the pitch angle (with the latter being uncontrollable as long as the roll was applied to make the matters worse). I considered it a bug but the particular behaviour was present also in the real simulator indicating that at least the yaw-pitch-roll controll was more less working in my simulator.

As far as it comes for other problems: the main reason of failure was trying to make too complicated equations without ability to handle a really proper testing. What I ended up struggeling with was fighting with bugs which were reveling themselves out of the sudden in compontents I considered working. A lot of times I was unable to determine the reason behind some malfunctions, which also costed me quite a lot of time.

Also I understimated the level of the project - it is quite hard to reflect realistic plane behaviour. I should have opted for more simplificated environment and limit the amount of supported plane parts.

# Godot license
MIT
https://github.com/godotengine/godot/blob/master/LICENSE.txt
