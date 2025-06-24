using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class LevelProperties
{
	public class AirshipClam : AbstractLevelProperties<AirshipClam.State, AirshipClam.Pattern, AirshipClam.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected AirshipClam properties { get; private set; }

			public virtual void LevelInit(AirshipClam properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Spit = 0,
			Barnacles = 1,
			Uninitialized = 2
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Spit spit;

			public readonly Barnacles barnacles;

			public readonly ClamOut clamOut;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Spit spit, Barnacles barnacles, ClamOut clamOut)
				: base(healthTrigger, patterns, stateName)
			{
				this.spit = spit;
				this.barnacles = barnacles;
				this.clamOut = clamOut;
			}
		}

		public class Spit : AbstractLevelPropertyGroup
		{
			public readonly float movementSpeedScale;

			public readonly float bulletSpeed;

			public readonly float preShotDelay;

			public readonly string attackDelayString;

			public readonly float initialShotDelay;

			public Spit(float movementSpeedScale, float bulletSpeed, float preShotDelay, string attackDelayString, float initialShotDelay)
			{
				this.movementSpeedScale = movementSpeedScale;
				this.bulletSpeed = bulletSpeed;
				this.preShotDelay = preShotDelay;
				this.attackDelayString = attackDelayString;
				this.initialShotDelay = initialShotDelay;
			}
		}

		public class Barnacles : AbstractLevelPropertyGroup
		{
			public readonly float initialArcMovementX;

			public readonly float initialArcMovementY;

			public readonly float initialGravity;

			public readonly float parryArcMovementX;

			public readonly float parryArcMovementY;

			public readonly float parryGravity;

			public readonly float rollingSpeed;

			public readonly string typeString;

			public readonly string attackDelayString;

			public readonly float barnacleScale;

			public readonly MinMax attackDuration;

			public Barnacles(float initialArcMovementX, float initialArcMovementY, float initialGravity, float parryArcMovementX, float parryArcMovementY, float parryGravity, float rollingSpeed, string typeString, string attackDelayString, float barnacleScale, MinMax attackDuration)
			{
				this.initialArcMovementX = initialArcMovementX;
				this.initialArcMovementY = initialArcMovementY;
				this.initialGravity = initialGravity;
				this.parryArcMovementX = parryArcMovementX;
				this.parryArcMovementY = parryArcMovementY;
				this.parryGravity = parryGravity;
				this.rollingSpeed = rollingSpeed;
				this.typeString = typeString;
				this.attackDelayString = attackDelayString;
				this.barnacleScale = barnacleScale;
				this.attackDuration = attackDuration;
			}
		}

		public class ClamOut : AbstractLevelPropertyGroup
		{
			public readonly float bulletSpeed;

			public readonly float bulletRepeatDelay;

			public readonly string shotString;

			public readonly float bulletMainDelay;

			public readonly float preShotDelay;

			public ClamOut(float bulletSpeed, float bulletRepeatDelay, string shotString, float bulletMainDelay, float preShotDelay)
			{
				this.bulletSpeed = bulletSpeed;
				this.bulletRepeatDelay = bulletRepeatDelay;
				this.shotString = shotString;
				this.bulletMainDelay = bulletMainDelay;
				this.preShotDelay = preShotDelay;
			}
		}

		public AirshipClam(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 200f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "S":
				return Pattern.Spit;
			case "B":
				return Pattern.Barnacles;
			default:
				Debug.LogError("Pattern AirshipClam.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static AirshipClam GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main, new Spit(0f, 0f, 0f, string.Empty, 0f), new Barnacles(0f, 0f, 0f, 0f, 0f, 0f, 0f, string.Empty, string.Empty, 0f, new MinMax(0f, 1f)), new ClamOut(0f, 0f, string.Empty, 0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 200;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] { Pattern.Barnacles } }, States.Main, new Spit(1.3f, 450f, 1f, "10,5,8,7,15", 2f), new Barnacles(140f, 650f, -24f, 130f, 850f, -20f, 300f, "R,P,R,P,P,R,R,P", "3.5,3.8,3.7,4.2,3.7", 2f, new MinMax(3f, 4f)), new ClamOut(800f, 1f, "4,3,5,4,3,4,2", 1.5f, 0.5f)));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main, new Spit(0f, 0f, 0f, string.Empty, 0f), new Barnacles(0f, 0f, 0f, 0f, 0f, 0f, 0f, string.Empty, string.Empty, 0f, new MinMax(0f, 1f)), new ClamOut(0f, 0f, string.Empty, 0f, 0f)));
				break;
			}
			return new AirshipClam(hp, goalTimes, list.ToArray());
		}
	}

	public class AirshipCrab : AbstractLevelProperties<AirshipCrab.State, AirshipCrab.Pattern, AirshipCrab.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected AirshipCrab properties { get; private set; }

			public virtual void LevelInit(AirshipCrab properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Main main;

			public readonly Barnicles barnicles;

			public readonly Gems gems;

			public readonly Bubbles bubbles;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Main main, Barnicles barnicles, Gems gems, Bubbles bubbles)
				: base(healthTrigger, patterns, stateName)
			{
				this.main = main;
				this.barnicles = barnicles;
				this.gems = gems;
				this.bubbles = bubbles;
			}
		}

		public class Main : AbstractLevelPropertyGroup
		{
			public readonly float openCrabOffsetY;

			public Main(float openCrabOffsetY)
			{
				this.openCrabOffsetY = openCrabOffsetY;
			}
		}

		public class Barnicles : AbstractLevelPropertyGroup
		{
			public readonly float bulletSpeed;

			public readonly float shotDelay;

			public readonly float hesitate;

			public readonly float barnicleAmount;

			public readonly float barnicleOffsetX;

			public readonly float barnicleOffsetY;

			public Barnicles(float bulletSpeed, float shotDelay, float hesitate, float barnicleAmount, float barnicleOffsetX, float barnicleOffsetY)
			{
				this.bulletSpeed = bulletSpeed;
				this.shotDelay = shotDelay;
				this.hesitate = hesitate;
				this.barnicleAmount = barnicleAmount;
				this.barnicleOffsetX = barnicleOffsetX;
				this.barnicleOffsetY = barnicleOffsetY;
			}
		}

		public class Gems : AbstractLevelPropertyGroup
		{
			public readonly float bulletSpeed;

			public readonly string[] angleString;

			public readonly string[] gemReleaseDelay;

			public readonly float gemAmount;

			public readonly float gemATKAmount;

			public readonly float gemOffsetX;

			public readonly float gemOffsetY;

			public readonly float gemHoldDuration;

			public Gems(float bulletSpeed, string[] angleString, string[] gemReleaseDelay, float gemAmount, float gemATKAmount, float gemOffsetX, float gemOffsetY, float gemHoldDuration)
			{
				this.bulletSpeed = bulletSpeed;
				this.angleString = angleString;
				this.gemReleaseDelay = gemReleaseDelay;
				this.gemAmount = gemAmount;
				this.gemATKAmount = gemATKAmount;
				this.gemOffsetX = gemOffsetX;
				this.gemOffsetY = gemOffsetY;
				this.gemHoldDuration = gemHoldDuration;
			}
		}

		public class Bubbles : AbstractLevelPropertyGroup
		{
			public readonly float bubbleSpeed;

			public readonly string[] bubbleCount;

			public readonly float bubbleRepeatDelay;

			public readonly float bubbleMainDelay;

			public readonly float openTimer;

			public readonly float sinWaveStrength;

			public Bubbles(float bubbleSpeed, string[] bubbleCount, float bubbleRepeatDelay, float bubbleMainDelay, float openTimer, float sinWaveStrength)
			{
				this.bubbleSpeed = bubbleSpeed;
				this.bubbleCount = bubbleCount;
				this.bubbleRepeatDelay = bubbleRepeatDelay;
				this.bubbleMainDelay = bubbleMainDelay;
				this.openTimer = openTimer;
				this.sinWaveStrength = sinWaveStrength;
			}
		}

		public AirshipCrab(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 800f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.83f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.63f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.38f));
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern AirshipCrab.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static AirshipCrab GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Main(0f), new Barnicles(0f, 0f, 0f, 0f, 0f, 0f), new Gems(0f, new string[0], new string[0], 0f, 0f, 0f, 0f, 0f), new Bubbles(0f, new string[0], 0f, 0f, 0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 800;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Main(120f), new Barnicles(400f, 1.2f, 2.2f, 4f, 50f, 90f), new Gems(600f, new string[4] { "50,205,315,64,235,185", "190,35,300,60,190,215", "55,245,145,155,335,35", "240,35,115,300,55,330" }, new string[1] { "1.2" }, 4f, 1f, 0f, 100f, 3.8f), new Bubbles(320f, new string[4] { "2,1,2,3,2,3,1", "1,2,1,3,2,2", "2,1,3,2,1", "3,1,1,2,2" }, 1f, 1.8f, 10f, 0.35f)));
				list.Add(new State(0.83f, new Pattern[1][] { new Pattern[0] }, States.Generic, new Main(120f), new Barnicles(400f, 1.2f, 2.2f, 4f, 50f, 90f), new Gems(600f, new string[1] { "43,190,305,64,235,185" }, new string[1] { "1" }, 4f, 2f, 0f, 90f, 4f), new Bubbles(320f, new string[4] { "2,1,2,3,2,3,1", "1,2,1,3,2,2", "2,1,3,2,1", "3,1,1,2,2" }, 1f, 1.8f, 10f, 0.35f)));
				list.Add(new State(0.63f, new Pattern[1][] { new Pattern[0] }, States.Generic, new Main(120f), new Barnicles(400f, 1.2f, 2.2f, 4f, 50f, 90f), new Gems(650f, new string[1] { "60,220,299,74,265,30" }, new string[1] { "0.8" }, 4f, 3f, 0f, 90f, 4.2f), new Bubbles(320f, new string[4] { "2,1,2,3,2,3,1", "1,2,1,3,2,2", "2,1,3,2,1", "3,1,1,2,2" }, 1f, 1.8f, 10f, 0.35f)));
				list.Add(new State(0.38f, new Pattern[1][] { new Pattern[0] }, States.Generic, new Main(120f), new Barnicles(400f, 1.2f, 2.2f, 4f, 50f, 90f), new Gems(660f, new string[1] { "50,205,315,64,235,35" }, new string[1] { "0.6" }, 4f, 4f, 0f, 90f, 4.2f), new Bubbles(320f, new string[4] { "2,1,2,3,2,3,1", "1,2,1,3,2,2", "2,1,3,2,1", "3,1,1,2,2" }, 1f, 1.8f, 10f, 0.35f)));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Main(0f), new Barnicles(0f, 0f, 0f, 0f, 0f, 0f), new Gems(0f, new string[0], new string[0], 0f, 0f, 0f, 0f, 0f), new Bubbles(0f, new string[0], 0f, 0f, 0f, 0f)));
				break;
			}
			return new AirshipCrab(hp, goalTimes, list.ToArray());
		}
	}

	public class AirshipJelly : AbstractLevelProperties<AirshipJelly.State, AirshipJelly.Pattern, AirshipJelly.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected AirshipJelly properties { get; private set; }

			public virtual void LevelInit(AirshipJelly properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Main = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Main main;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Main main)
				: base(healthTrigger, patterns, stateName)
			{
				this.main = main;
			}
		}

		public class Main : AbstractLevelPropertyGroup
		{
			public readonly float time;

			public readonly float orbDelay;

			public readonly float hurtDelay;

			public readonly float parryDamage;

			public readonly MinMax speed;

			public Main(float time, float orbDelay, float hurtDelay, float parryDamage, MinMax speed)
			{
				this.time = time;
				this.orbDelay = orbDelay;
				this.hurtDelay = hurtDelay;
				this.parryDamage = parryDamage;
				this.speed = speed;
			}
		}

		public AirshipJelly(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 2000f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "M")
			{
				return Pattern.Main;
			}
			Debug.LogError("Pattern AirshipJelly.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static AirshipJelly GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main, new Main(0f, 0f, 0f, 0f, new MinMax(0f, 1f))));
				break;
			case Level.Mode.Normal:
				hp = 2000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Main(2f, 3f, 2f, 380f, new MinMax(400f, 860f))));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main, new Main(0f, 0f, 0f, 0f, new MinMax(0f, 1f))));
				break;
			}
			return new AirshipJelly(hp, goalTimes, list.ToArray());
		}
	}

	public class AirshipStork : AbstractLevelProperties<AirshipStork.State, AirshipStork.Pattern, AirshipStork.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected AirshipStork properties { get; private set; }

			public virtual void LevelInit(AirshipStork properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Main main;

			public readonly SpiralShot spiralShot;

			public readonly Babies babies;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Main main, SpiralShot spiralShot, Babies babies)
				: base(healthTrigger, patterns, stateName)
			{
				this.main = main;
				this.spiralShot = spiralShot;
				this.babies = babies;
			}
		}

		public class Main : AbstractLevelPropertyGroup
		{
			public readonly float parryDamage;

			public readonly float movementSpeed;

			public readonly string[] leftMovementTime;

			public readonly float headHeight;

			public readonly float pinkDurationOff;

			public Main(float parryDamage, float movementSpeed, string[] leftMovementTime, float headHeight, float pinkDurationOff)
			{
				this.parryDamage = parryDamage;
				this.movementSpeed = movementSpeed;
				this.leftMovementTime = leftMovementTime;
				this.headHeight = headHeight;
				this.pinkDurationOff = pinkDurationOff;
			}
		}

		public class SpiralShot : AbstractLevelPropertyGroup
		{
			public readonly float movementSpeed;

			public readonly float spiralRate;

			public readonly string[] pinkString;

			public readonly string[] shotDelayString;

			public readonly string[] spiralDirection;

			public SpiralShot(float movementSpeed, float spiralRate, string[] pinkString, string[] shotDelayString, string[] spiralDirection)
			{
				this.movementSpeed = movementSpeed;
				this.spiralRate = spiralRate;
				this.pinkString = pinkString;
				this.shotDelayString = shotDelayString;
				this.spiralDirection = spiralDirection;
			}
		}

		public class Babies : AbstractLevelPropertyGroup
		{
			public readonly float HP;

			public readonly string[] YVelocityRange;

			public readonly string[] babyDelayString;

			public readonly float highVerticalSpeed;

			public readonly float highHorizontalSpeed;

			public readonly float highGravity;

			public readonly float lowVerticalSpeed;

			public readonly float lowHorizontalSpeed;

			public readonly float lowGravity;

			public readonly string[] patternString;

			public Babies(float HP, string[] YVelocityRange, string[] babyDelayString, float highVerticalSpeed, float highHorizontalSpeed, float highGravity, float lowVerticalSpeed, float lowHorizontalSpeed, float lowGravity, string[] patternString)
			{
				this.HP = HP;
				this.YVelocityRange = YVelocityRange;
				this.babyDelayString = babyDelayString;
				this.highVerticalSpeed = highVerticalSpeed;
				this.highHorizontalSpeed = highHorizontalSpeed;
				this.highGravity = highGravity;
				this.lowVerticalSpeed = lowVerticalSpeed;
				this.lowHorizontalSpeed = lowHorizontalSpeed;
				this.lowGravity = lowGravity;
				this.patternString = patternString;
			}
		}

		public AirshipStork(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 2000f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern AirshipStork.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static AirshipStork GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Main(0f, 0f, new string[0], 0f, 0f), new SpiralShot(0f, 0f, new string[0], new string[0], new string[0]), new Babies(0f, new string[0], new string[0], 0f, 0f, 0f, 0f, 0f, 0f, new string[0])));
				break;
			case Level.Mode.Normal:
				hp = 2000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Main(400f, 180f, new string[1] { ".5,.4,.6,.7" }, 65f, 3f), new SpiralShot(95f, 0.36f, new string[1] { "P,P,R,R,R,P,P,P,R,P,P" }, new string[1] { "3.4,4,3,5,4,3,2.5" }, new string[1] { "1,1,2,1,2,2,1,2" }), new Babies(12f, new string[1] { "200,600,400,500,100" }, new string[1] { "3,4,5,4" }, 3400f, 600f, 7000f, 2600f, 450f, 7000f, new string[1] { "HJ,HJ,LJ,D0.5,LJ" })));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Main(0f, 0f, new string[0], 0f, 0f), new SpiralShot(0f, 0f, new string[0], new string[0], new string[0]), new Babies(0f, new string[0], new string[0], 0f, 0f, 0f, 0f, 0f, 0f, new string[0])));
				break;
			}
			return new AirshipStork(hp, goalTimes, list.ToArray());
		}
	}

	public class Baroness : AbstractLevelProperties<Baroness.State, Baroness.Pattern, Baroness.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Baroness properties { get; private set; }

			public virtual void LevelInit(Baroness properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			Chase = 2
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly BaronessVonBonbon baronessVonBonbon;

			public readonly Open open;

			public readonly Jellybeans jellybeans;

			public readonly Gumball gumball;

			public readonly Waffle waffle;

			public readonly CandyCorn candyCorn;

			public readonly Cupcake cupcake;

			public readonly Jawbreaker jawbreaker;

			public readonly Peppermint peppermint;

			public readonly Platform platform;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, BaronessVonBonbon baronessVonBonbon, Open open, Jellybeans jellybeans, Gumball gumball, Waffle waffle, CandyCorn candyCorn, Cupcake cupcake, Jawbreaker jawbreaker, Peppermint peppermint, Platform platform)
				: base(healthTrigger, patterns, stateName)
			{
				this.baronessVonBonbon = baronessVonBonbon;
				this.open = open;
				this.jellybeans = jellybeans;
				this.gumball = gumball;
				this.waffle = waffle;
				this.candyCorn = candyCorn;
				this.cupcake = cupcake;
				this.jawbreaker = jawbreaker;
				this.peppermint = peppermint;
				this.platform = platform;
			}
		}

		public class BaronessVonBonbon : AbstractLevelPropertyGroup
		{
			public readonly int HP;

			public readonly float miniBossStart;

			public readonly string[] timeString;

			public readonly MinMax attackDelay;

			public readonly MinMax attackCount;

			public readonly int projectileHP;

			public readonly float projectileRotationSpeed;

			public readonly int projectileSpeed;

			public readonly float finalProjectileSpeed;

			public readonly float finalProjectileMoveDuration;

			public readonly float finalProjectileRedirectDelay;

			public readonly float finalProjectileRedirectCount;

			public readonly MinMax finalProjectileAttackDelayRange;

			public readonly float finalProjectileInitialDelay;

			public readonly string finalProjectileHeadToss;

			public BaronessVonBonbon(int HP, float miniBossStart, string[] timeString, MinMax attackDelay, MinMax attackCount, int projectileHP, float projectileRotationSpeed, int projectileSpeed, float finalProjectileSpeed, float finalProjectileMoveDuration, float finalProjectileRedirectDelay, float finalProjectileRedirectCount, MinMax finalProjectileAttackDelayRange, float finalProjectileInitialDelay, string finalProjectileHeadToss)
			{
				this.HP = HP;
				this.miniBossStart = miniBossStart;
				this.timeString = timeString;
				this.attackDelay = attackDelay;
				this.attackCount = attackCount;
				this.projectileHP = projectileHP;
				this.projectileRotationSpeed = projectileRotationSpeed;
				this.projectileSpeed = projectileSpeed;
				this.finalProjectileSpeed = finalProjectileSpeed;
				this.finalProjectileMoveDuration = finalProjectileMoveDuration;
				this.finalProjectileRedirectDelay = finalProjectileRedirectDelay;
				this.finalProjectileRedirectCount = finalProjectileRedirectCount;
				this.finalProjectileAttackDelayRange = finalProjectileAttackDelayRange;
				this.finalProjectileInitialDelay = finalProjectileInitialDelay;
				this.finalProjectileHeadToss = finalProjectileHeadToss;
			}
		}

		public class Open : AbstractLevelPropertyGroup
		{
			public readonly int miniBossAmount;

			public readonly string[] miniBossString;

			public Open(int miniBossAmount, string[] miniBossString)
			{
				this.miniBossAmount = miniBossAmount;
				this.miniBossString = miniBossString;
			}
		}

		public class Jellybeans : AbstractLevelPropertyGroup
		{
			public readonly int HP;

			public readonly float movementSpeed;

			public readonly float heightDefault;

			public readonly float jumpSpeed;

			public readonly MinMax jumpHeight;

			public readonly float afterJumpDuration;

			public readonly string[] typeArray;

			public readonly MinMax spawnDelay;

			public readonly float startingPoint;

			public readonly float spawnDelayChangePercentage;

			public Jellybeans(int HP, float movementSpeed, float heightDefault, float jumpSpeed, MinMax jumpHeight, float afterJumpDuration, string[] typeArray, MinMax spawnDelay, float startingPoint, float spawnDelayChangePercentage)
			{
				this.HP = HP;
				this.movementSpeed = movementSpeed;
				this.heightDefault = heightDefault;
				this.jumpSpeed = jumpSpeed;
				this.jumpHeight = jumpHeight;
				this.afterJumpDuration = afterJumpDuration;
				this.typeArray = typeArray;
				this.spawnDelay = spawnDelay;
				this.startingPoint = startingPoint;
				this.spawnDelayChangePercentage = spawnDelayChangePercentage;
			}
		}

		public class Gumball : AbstractLevelPropertyGroup
		{
			public readonly int HP;

			public readonly float gumballMovementSpeed;

			public readonly float gumballDeathSpeed;

			public readonly MinMax gumballAttackDurationOffRange;

			public readonly MinMax gumballAttackDurationOnRange;

			public readonly float gravity;

			public readonly MinMax velocityX;

			public readonly float rateOfFire;

			public readonly MinMax velocityY;

			public readonly MinMax offsetX;

			public Gumball(int HP, float gumballMovementSpeed, float gumballDeathSpeed, MinMax gumballAttackDurationOffRange, MinMax gumballAttackDurationOnRange, float gravity, MinMax velocityX, float rateOfFire, MinMax velocityY, MinMax offsetX)
			{
				this.HP = HP;
				this.gumballMovementSpeed = gumballMovementSpeed;
				this.gumballDeathSpeed = gumballDeathSpeed;
				this.gumballAttackDurationOffRange = gumballAttackDurationOffRange;
				this.gumballAttackDurationOnRange = gumballAttackDurationOnRange;
				this.gravity = gravity;
				this.velocityX = velocityX;
				this.rateOfFire = rateOfFire;
				this.velocityY = velocityY;
				this.offsetX = offsetX;
			}
		}

		public class Waffle : AbstractLevelPropertyGroup
		{
			public readonly int HP;

			public readonly float movementSpeed;

			public readonly float anticipation;

			public readonly MinMax attackDelayRange;

			public readonly float explodeSpeed;

			public readonly float explodeTwoDuration;

			public readonly float explodeDistance;

			public readonly float explodeReturnSpeed;

			public readonly float XAxisSpeed;

			public readonly float pivotPointMoveAmount;

			public Waffle(int HP, float movementSpeed, float anticipation, MinMax attackDelayRange, float explodeSpeed, float explodeTwoDuration, float explodeDistance, float explodeReturnSpeed, float XAxisSpeed, float pivotPointMoveAmount)
			{
				this.HP = HP;
				this.movementSpeed = movementSpeed;
				this.anticipation = anticipation;
				this.attackDelayRange = attackDelayRange;
				this.explodeSpeed = explodeSpeed;
				this.explodeTwoDuration = explodeTwoDuration;
				this.explodeDistance = explodeDistance;
				this.explodeReturnSpeed = explodeReturnSpeed;
				this.XAxisSpeed = XAxisSpeed;
				this.pivotPointMoveAmount = pivotPointMoveAmount;
			}
		}

		public class CandyCorn : AbstractLevelPropertyGroup
		{
			public readonly int HP;

			public readonly float movementSpeed;

			public readonly string[] changeLevelString;

			public readonly float centerPosition;

			public readonly float deathMoveSpeed;

			public readonly float deathAcceleration;

			public readonly MinMax miniCornSpawnDelay;

			public readonly int miniCornHP;

			public readonly float miniCornMovementSpeed;

			public readonly bool spawnMinis;

			public CandyCorn(int HP, float movementSpeed, string[] changeLevelString, float centerPosition, float deathMoveSpeed, float deathAcceleration, MinMax miniCornSpawnDelay, int miniCornHP, float miniCornMovementSpeed, bool spawnMinis)
			{
				this.HP = HP;
				this.movementSpeed = movementSpeed;
				this.changeLevelString = changeLevelString;
				this.centerPosition = centerPosition;
				this.deathMoveSpeed = deathMoveSpeed;
				this.deathAcceleration = deathAcceleration;
				this.miniCornSpawnDelay = miniCornSpawnDelay;
				this.miniCornHP = miniCornHP;
				this.miniCornMovementSpeed = miniCornMovementSpeed;
				this.spawnMinis = spawnMinis;
			}
		}

		public class Cupcake : AbstractLevelPropertyGroup
		{
			public readonly int HP;

			public readonly string[] XSpeedString;

			public readonly float hold;

			public readonly float splashOriginalOffset;

			public readonly float splashOffset;

			public readonly bool projectileOn;

			public Cupcake(int HP, string[] XSpeedString, float hold, float splashOriginalOffset, float splashOffset, bool projectileOn)
			{
				this.HP = HP;
				this.XSpeedString = XSpeedString;
				this.hold = hold;
				this.splashOriginalOffset = splashOriginalOffset;
				this.splashOffset = splashOffset;
				this.projectileOn = projectileOn;
			}
		}

		public class Jawbreaker : AbstractLevelPropertyGroup
		{
			public readonly int jawbreakerMinis;

			public readonly float jawbreakerMiniSpace;

			public readonly float jawbreakerHomeDuration;

			public readonly int jawbreakerHomingHP;

			public readonly float jawbreakerHomingSpeed;

			public readonly float jawbreakerHomingRotation;

			public Jawbreaker(int jawbreakerMinis, float jawbreakerMiniSpace, float jawbreakerHomeDuration, int jawbreakerHomingHP, float jawbreakerHomingSpeed, float jawbreakerHomingRotation)
			{
				this.jawbreakerMinis = jawbreakerMinis;
				this.jawbreakerMiniSpace = jawbreakerMiniSpace;
				this.jawbreakerHomeDuration = jawbreakerHomeDuration;
				this.jawbreakerHomingHP = jawbreakerHomingHP;
				this.jawbreakerHomingSpeed = jawbreakerHomingSpeed;
				this.jawbreakerHomingRotation = jawbreakerHomingRotation;
			}
		}

		public class Peppermint : AbstractLevelPropertyGroup
		{
			public readonly float peppermintSpeed;

			public readonly MinMax peppermintSpawnDurationRange;

			public Peppermint(float peppermintSpeed, MinMax peppermintSpawnDurationRange)
			{
				this.peppermintSpeed = peppermintSpeed;
				this.peppermintSpawnDurationRange = peppermintSpawnDurationRange;
			}
		}

		public class Platform : AbstractLevelPropertyGroup
		{
			public readonly float LeftBoundaryOffset;

			public readonly float RightBoundaryOffset;

			public readonly float YPosition;

			public Platform(float LeftBoundaryOffset, float RightBoundaryOffset, float YPosition)
			{
				this.LeftBoundaryOffset = LeftBoundaryOffset;
				this.RightBoundaryOffset = RightBoundaryOffset;
				this.YPosition = YPosition;
			}
		}

		public Baroness(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 400f;
				break;
			case Level.Mode.Normal:
				timeline.health = 430f;
				break;
			case Level.Mode.Hard:
				timeline.health = 530f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern Baroness.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static Baroness GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 400;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new BaronessVonBonbon(0, 5f, new string[1] { "4,6,8,4,9" }, new MinMax(2.4f, 3.7f), new MinMax(1f, 1f), 3, 100f, 480, 650f, 0.7f, 0.5f, 5f, new MinMax(4.5f, 7.5f), 1.5f, "H,H,D:2,H,H,D:2.5,H,H,D:1.5"), new Open(3, new string[2] { "2,5,1,3", "5,1,3,2" }), new Jellybeans(7, 300f, 60f, 550f, new MinMax(-101f, -100f), 1f, new string[1] { "P,P,P,R,P,P,P,P,R" }, new MinMax(4.6f, 6.4f), 2f, 5f), new Gumball(270, 3.7f, 600f, new MinMax(9000f, 9005f), new MinMax(0f, 0f), 850f, new MinMax(-290f, 290f), 0.5f, new MinMax(-150f, 150f), new MinMax(0f, 205f)), new Waffle(250, 1.8f, 1f, new MinMax(9000f, 9005f), 3f, 0f, 400f, 4f, 150f, 430f), new CandyCorn(225, 505f, new string[2] { "Y,Y,N,Y,Y,N,Y,N,Y,N", "N,Y,N,N,Y,Y,Y,Y,N" }, -150f, 600f, 2f, new MinMax(2000f, 2002f), 10, 70f, false), new Cupcake(185, new string[1] { "1000, 1200, 1500, 1300,1100,1050,1400" }, 0.7f, 200f, 75f, false), new Jawbreaker(0, 235f, 5f, 180, 300f, 1.9f), new Peppermint(330f, new MinMax(4f, 7.7f)), new Platform(180f, 400f, -15f)));
				break;
			case Level.Mode.Normal:
				hp = 430;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new BaronessVonBonbon(0, 3f, new string[2] { "6,5,7,5.5,7.5,6,6,5", "5,8,5.5,7,5,6,8" }, new MinMax(4f, 6.5f), new MinMax(1f, 1f), 3, 100f, 250, 565f, 0.7f, 0.5f, 5f, new MinMax(7.5f, 8.1f), 0.2f, "H,H,D:2,H,H,D:2.5,H,H,D:1.5"), new Open(3, new string[3] { "1,2,3,4,5", "1,3,5,2,4", "5,4,2,3,1" }), new Jellybeans(7, 300f, 60f, 550f, new MinMax(-101f, -100f), 1f, new string[2] { "R,R,P,R,P,R,P,P", "P,R,P,R,R,P,R,P" }, new MinMax(4.8f, 6.6f), 2f, 5f), new Gumball(270, 2.3f, 500f, new MinMax(2f, 3.5f), new MinMax(3.5f, 5f), 950f, new MinMax(-255f, 450f), 0.23f, new MinMax(600f, 800f), new MinMax(330f, 600f)), new Waffle(250, 1.8f, 1f, new MinMax(2f, 5f), 3f, 0.001f, 425f, 4f, 110f, 380f), new CandyCorn(225, 465f, new string[1] { "Y,N,Y,Y,N,Y,N,Y,Y,N,N,Y,N,Y,Y" }, -150f, 600f, 2f, new MinMax(1.5f, 2.3f), 10, 80f, true), new Cupcake(185, new string[2] { "1000, 1200, 1500, 1300,1100,1050,1400", "1150,1400,1300,1000,1200,1100" }, 1.25f, 200f, 75f, true), new Jawbreaker(1, 245f, 5f, 180, 265f, 2.1f), new Peppermint(325f, new MinMax(4f, 7.7f)), new Platform(180f, 400f, -15f)));
				break;
			case Level.Mode.Hard:
				hp = 530;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new BaronessVonBonbon(0, 3f, new string[1] { "5,7,9,7,8,6,7" }, new MinMax(2f, 3.2f), new MinMax(1f, 1f), 3, 100f, 300, 620f, 0.61f, 0.45f, 6f, new MinMax(4.3f, 6.7f), 1.5f, "H,H,D:2.4,H,H,D:2.7,H,H,D:3"), new Open(3, new string[1] { "1,2,3,4,5" }), new Jellybeans(7, 315f, 60f, 550f, new MinMax(-101f, -100f), 1f, new string[3] { "R,R,P,R,P,P,R,R,P", "R,P,R,P,R,R,P", "P,P,R,R,P,R,R,P" }, new MinMax(5f, 6.8f), 1f, 5f), new Gumball(320, 1.85f, 500f, new MinMax(2f, 3.5f), new MinMax(4.3f, 5.7f), 950f, new MinMax(-260f, 450f), 0.16f, new MinMax(600f, 850f), new MinMax(330f, 600f)), new Waffle(305, 1.8f, 1f, new MinMax(2f, 5f), 3f, 1f, 450f, 4f, 130f, 380f), new CandyCorn(250, 535f, new string[2] { "Y,N,Y,Y,N,N,Y,Y,Y,N,Y,N,N", "Y,Y,Y,N,Y,N,N,Y,N,N,Y,Y,Y" }, -150f, 600f, 2f, new MinMax(1.3f, 2f), 10, 80f, true), new Cupcake(235, new string[2] { "1000, 1200, 1500, 1300,1100,1050,1400", "950,1150,1400,1300,1000,1000,1500" }, 0.85f, 200f, 75f, true), new Jawbreaker(2, 230f, 5f, 220, 265f, 1.8f), new Peppermint(340f, new MinMax(3.5f, 6.7f)), new Platform(180f, 400f, -15f)));
				break;
			}
			return new Baroness(hp, goalTimes, list.ToArray());
		}
	}

	public class Bat : AbstractLevelProperties<Bat.State, Bat.Pattern, Bat.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Bat properties { get; private set; }

			public virtual void LevelInit(Bat properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			Coffin = 2,
			Wolf = 3
		}

		public enum Pattern
		{
			Bouncer = 0,
			Lightning = 1,
			Uninitialized = 2
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Movement movement;

			public readonly BatBouncer batBouncer;

			public readonly Goblins goblins;

			public readonly BatLightning batLightning;

			public readonly MiniBats miniBats;

			public readonly Pentagrams pentagrams;

			public readonly CrossToss crossToss;

			public readonly WolfFire wolfFire;

			public readonly WolfSoul wolfSoul;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Movement movement, BatBouncer batBouncer, Goblins goblins, BatLightning batLightning, MiniBats miniBats, Pentagrams pentagrams, CrossToss crossToss, WolfFire wolfFire, WolfSoul wolfSoul)
				: base(healthTrigger, patterns, stateName)
			{
				this.movement = movement;
				this.batBouncer = batBouncer;
				this.goblins = goblins;
				this.batLightning = batLightning;
				this.miniBats = miniBats;
				this.pentagrams = pentagrams;
				this.crossToss = crossToss;
				this.wolfFire = wolfFire;
				this.wolfSoul = wolfSoul;
			}
		}

		public class Movement : AbstractLevelPropertyGroup
		{
			public readonly float movementSpeed;

			public readonly float startPosY;

			public Movement(float movementSpeed, float startPosY)
			{
				this.movementSpeed = movementSpeed;
				this.startPosY = startPosY;
			}
		}

		public class BatBouncer : AbstractLevelPropertyGroup
		{
			public readonly float mainBounceSpeed;

			public readonly float pinkBounceSpeed;

			public readonly float acceleration;

			public readonly float breakCounter;

			public readonly string[] delayBeforeAttackString;

			public readonly float stopDelay;

			public readonly string[] bounceAngleString;

			public readonly float hesitate;

			public BatBouncer(float mainBounceSpeed, float pinkBounceSpeed, float acceleration, float breakCounter, string[] delayBeforeAttackString, float stopDelay, string[] bounceAngleString, float hesitate)
			{
				this.mainBounceSpeed = mainBounceSpeed;
				this.pinkBounceSpeed = pinkBounceSpeed;
				this.acceleration = acceleration;
				this.breakCounter = breakCounter;
				this.delayBeforeAttackString = delayBeforeAttackString;
				this.stopDelay = stopDelay;
				this.bounceAngleString = bounceAngleString;
				this.hesitate = hesitate;
			}
		}

		public class Goblins : AbstractLevelPropertyGroup
		{
			public readonly bool Enabled;

			public readonly float HP;

			public readonly float runSpeed;

			public readonly string[] appearDelayString;

			public readonly string[] entranceString;

			public readonly MinMax shooterOccuranceRange;

			public readonly float timeBeforeShoot;

			public readonly float initalShotDelay;

			public readonly float bulletSpeed;

			public readonly float shooterHold;

			public Goblins(bool Enabled, float HP, float runSpeed, string[] appearDelayString, string[] entranceString, MinMax shooterOccuranceRange, float timeBeforeShoot, float initalShotDelay, float bulletSpeed, float shooterHold)
			{
				this.Enabled = Enabled;
				this.HP = HP;
				this.runSpeed = runSpeed;
				this.appearDelayString = appearDelayString;
				this.entranceString = entranceString;
				this.shooterOccuranceRange = shooterOccuranceRange;
				this.timeBeforeShoot = timeBeforeShoot;
				this.initalShotDelay = initalShotDelay;
				this.bulletSpeed = bulletSpeed;
				this.shooterHold = shooterHold;
			}
		}

		public class BatLightning : AbstractLevelPropertyGroup
		{
			public readonly float cloudCount;

			public readonly float cloudDistance;

			public readonly string[] centerOffset;

			public readonly string[] initialAttackDelayString;

			public readonly float cloudWarning;

			public readonly float lightningOnDuration;

			public readonly float cloudHeight;

			public readonly float hesitate;

			public BatLightning(float cloudCount, float cloudDistance, string[] centerOffset, string[] initialAttackDelayString, float cloudWarning, float lightningOnDuration, float cloudHeight, float hesitate)
			{
				this.cloudCount = cloudCount;
				this.cloudDistance = cloudDistance;
				this.centerOffset = centerOffset;
				this.initialAttackDelayString = initialAttackDelayString;
				this.cloudWarning = cloudWarning;
				this.lightningOnDuration = lightningOnDuration;
				this.cloudHeight = cloudHeight;
				this.hesitate = hesitate;
			}
		}

		public class MiniBats : AbstractLevelPropertyGroup
		{
			public readonly float HP;

			public readonly float initialAttackDelay;

			public readonly float speedX;

			public readonly float speedY;

			public readonly float yMinMax;

			public readonly string[] batAngleString;

			public readonly float delay;

			public MiniBats(float HP, float initialAttackDelay, float speedX, float speedY, float yMinMax, string[] batAngleString, float delay)
			{
				this.HP = HP;
				this.initialAttackDelay = initialAttackDelay;
				this.speedX = speedX;
				this.speedY = speedY;
				this.yMinMax = yMinMax;
				this.batAngleString = batAngleString;
				this.delay = delay;
			}
		}

		public class Pentagrams : AbstractLevelPropertyGroup
		{
			public readonly float xSpeed;

			public readonly float ySpeed;

			public readonly string[] pentagramDelayString;

			public readonly float pentagramSize;

			public Pentagrams(float xSpeed, float ySpeed, string[] pentagramDelayString, float pentagramSize)
			{
				this.xSpeed = xSpeed;
				this.ySpeed = ySpeed;
				this.pentagramDelayString = pentagramDelayString;
				this.pentagramSize = pentagramSize;
			}
		}

		public class CrossToss : AbstractLevelPropertyGroup
		{
			public readonly float projectileSpeed;

			public readonly string[] attackCount;

			public readonly string[] crossDelayString;

			public CrossToss(float projectileSpeed, string[] attackCount, string[] crossDelayString)
			{
				this.projectileSpeed = projectileSpeed;
				this.attackCount = attackCount;
				this.crossDelayString = crossDelayString;
			}
		}

		public class WolfFire : AbstractLevelPropertyGroup
		{
			public readonly float bulletSpeed;

			public readonly float bulletDelay;

			public readonly float bulletAimCount;

			public WolfFire(float bulletSpeed, float bulletDelay, float bulletAimCount)
			{
				this.bulletSpeed = bulletSpeed;
				this.bulletDelay = bulletDelay;
				this.bulletAimCount = bulletAimCount;
			}
		}

		public class WolfSoul : AbstractLevelPropertyGroup
		{
			public readonly float regularSize;

			public readonly float attackSize;

			public readonly float attackDuration;

			public readonly string[] floatUpDuration;

			public readonly float floatSpeed;

			public readonly float floatWarningDuration;

			public readonly float homingSpeed;

			public readonly float homingRotation;

			public WolfSoul(float regularSize, float attackSize, float attackDuration, string[] floatUpDuration, float floatSpeed, float floatWarningDuration, float homingSpeed, float homingRotation)
			{
				this.regularSize = regularSize;
				this.attackSize = attackSize;
				this.attackDuration = attackDuration;
				this.floatUpDuration = floatUpDuration;
				this.floatSpeed = floatSpeed;
				this.floatWarningDuration = floatWarningDuration;
				this.homingSpeed = homingSpeed;
				this.homingRotation = homingRotation;
			}
		}

		public Bat(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 800f;
				timeline.events.Add(new Level.Timeline.Event("Coffin", 0.53f));
				timeline.events.Add(new Level.Timeline.Event("Wolf", 0.2f));
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "B":
				return Pattern.Bouncer;
			case "L":
				return Pattern.Lightning;
			default:
				Debug.LogError("Pattern Bat.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static Bat GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Movement(0f, 0f), new BatBouncer(0f, 0f, 0f, 0f, new string[0], 0f, new string[0], 0f), new Goblins(false, 0f, 0f, new string[0], new string[0], new MinMax(0f, 1f), 0f, 0f, 0f, 0f), new BatLightning(0f, 0f, new string[0], new string[0], 0f, 0f, 0f, 0f), new MiniBats(0f, 0f, 0f, 0f, 0f, new string[0], 0f), new Pentagrams(0f, 0f, new string[0], 0f), new CrossToss(0f, new string[0], new string[0]), new WolfFire(0f, 0f, 0f), new WolfSoul(0f, 0f, 0f, new string[0], 0f, 0f, 0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 800;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[3]
				{
					Pattern.Bouncer,
					Pattern.Lightning,
					Pattern.Bouncer
				} }, States.Main, new Movement(300f, 160f), new BatBouncer(500f, 700f, 1f, 6f, new string[1] { "3,5,4,2,3.3" }, 1f, new string[1] { "240,228,215,230,236" }, 5f), new Goblins(true, 10f, 300f, new string[1] { "6,5,6,7,6,7,5,7" }, new string[1] { "R,R,L,R,L" }, new MinMax(1f, 1f), 2f, 1f, 300f, 1f), new BatLightning(4f, 330f, new string[1] { "50,0,100,-70" }, new string[1] { "7,8,9" }, 1f, 2f, 170f, 2f), new MiniBats(7f, 2f, 200f, 15f, 1.5f, new string[1] { "0,20,35,5,0,50,350" }, 1.3f), new Pentagrams(200f, 300f, new string[1] { "4,6,5,4.3,5.5,6.4" }, 0.5f), new CrossToss(400f, new string[1] { "2,3,2" }, new string[1] { "5,7,6,8" }), new WolfFire(330f, 0.8f, 5f), new WolfSoul(1f, 1.5f, 1f, new string[1] { "3,5,6,7" }, 100f, 1f, 230f, 1.6f)));
				list.Add(new State(0.53f, new Pattern[1][] { new Pattern[0] }, States.Coffin, new Movement(300f, 160f), new BatBouncer(500f, 700f, 1f, 6f, new string[1] { "3,5,4,2,3.3" }, 1f, new string[1] { "240,228,215,230,236" }, 5f), new Goblins(true, 1f, 300f, new string[1] { "1000,9000" }, new string[1] { "R,R,R,R,R,R,L" }, new MinMax(1f, 5f), 1f, 1f, 300f, 1f), new BatLightning(4f, 330f, new string[1] { "50,0,100,-70" }, new string[1] { "7,8,9" }, 1f, 2f, 170f, 2f), new MiniBats(7f, 2f, 200f, 15f, 1.5f, new string[1] { "0,20,35,5,0,50,350" }, 1.3f), new Pentagrams(200f, 300f, new string[1] { "4,6,5,4.3,5.5,6.4" }, 0.5f), new CrossToss(400f, new string[1] { "2,3,2" }, new string[1] { "5,7,6,8" }), new WolfFire(330f, 0.8f, 5f), new WolfSoul(1f, 1.5f, 1f, new string[1] { "3,5,6,7" }, 100f, 1f, 230f, 1.6f)));
				list.Add(new State(0.2f, new Pattern[1][] { new Pattern[0] }, States.Wolf, new Movement(300f, 160f), new BatBouncer(500f, 700f, 1f, 6f, new string[1] { "3,5,4,2,3.3" }, 1f, new string[1] { "240,228,215,230,236" }, 5f), new Goblins(true, 1f, 300f, new string[1] { "1000,9000" }, new string[1] { "R,R,R,R,R,R,L" }, new MinMax(1f, 5f), 1f, 1f, 300f, 1f), new BatLightning(4f, 330f, new string[1] { "50,0,100,-70" }, new string[1] { "7,8,9" }, 1f, 2f, 170f, 2f), new MiniBats(7f, 2f, 200f, 15f, 1.5f, new string[1] { "0,20,35,5,0,50,350" }, 1.3f), new Pentagrams(200f, 300f, new string[1] { "4,6,5,4.3,5.5,6.4" }, 0.5f), new CrossToss(400f, new string[1] { "2,3,2" }, new string[1] { "5,7,6,8" }), new WolfFire(330f, 0.8f, 5f), new WolfSoul(1f, 1.5f, 1f, new string[1] { "3,5,6,7" }, 100f, 1f, 230f, 1.6f)));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Movement(300f, 0f), new BatBouncer(400f, 600f, 1f, 6f, new string[1] { "3,5,4,2,3.3" }, 1f, new string[1] { "45,53,37,42" }, 5f), new Goblins(true, 20f, 200f, new string[1] { "4,3,5,6,4" }, new string[1] { "R,R,L,R,L" }, new MinMax(6f, 10f), 2f, 1f, 100f, 1f), new BatLightning(0f, 0f, new string[0], new string[0], 0f, 0f, 0f, 0f), new MiniBats(0f, 0f, 0f, 0f, 0f, new string[0], 0f), new Pentagrams(0f, 0f, new string[0], 0f), new CrossToss(0f, new string[0], new string[0]), new WolfFire(0f, 0f, 0f), new WolfSoul(0f, 0f, 0f, new string[0], 0f, 0f, 0f, 0f)));
				break;
			}
			return new Bat(hp, goalTimes, list.ToArray());
		}
	}

	public class Bee : AbstractLevelProperties<Bee.State, Bee.Pattern, Bee.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Bee properties { get; private set; }

			public virtual void LevelInit(Bee properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			Airplane = 2
		}

		public enum Pattern
		{
			BlackHole = 0,
			Chain = 1,
			Triangle = 2,
			Follower = 3,
			SecurityGuard = 4,
			Wing = 5,
			Turbine = 6,
			Uninitialized = 7
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Movement movement;

			public readonly Grunts grunts;

			public readonly BlackHole blackHole;

			public readonly Triangle triangle;

			public readonly Follower follower;

			public readonly Chain chain;

			public readonly SecurityGuard securityGuard;

			public readonly General general;

			public readonly WingSwipe wingSwipe;

			public readonly TurbineBlasters turbineBlasters;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Movement movement, Grunts grunts, BlackHole blackHole, Triangle triangle, Follower follower, Chain chain, SecurityGuard securityGuard, General general, WingSwipe wingSwipe, TurbineBlasters turbineBlasters)
				: base(healthTrigger, patterns, stateName)
			{
				this.movement = movement;
				this.grunts = grunts;
				this.blackHole = blackHole;
				this.triangle = triangle;
				this.follower = follower;
				this.chain = chain;
				this.securityGuard = securityGuard;
				this.general = general;
				this.wingSwipe = wingSwipe;
				this.turbineBlasters = turbineBlasters;
			}
		}

		public class Movement : AbstractLevelPropertyGroup
		{
			public readonly bool moving;

			public readonly float speed;

			public readonly int missingPlatforms;

			public Movement(bool moving, float speed, int missingPlatforms)
			{
				this.moving = moving;
				this.speed = speed;
				this.missingPlatforms = missingPlatforms;
			}
		}

		public class Grunts : AbstractLevelPropertyGroup
		{
			public readonly bool active;

			public readonly int health;

			public readonly string[] entrancePoints;

			public readonly float speed;

			public readonly float delay;

			public Grunts(bool active, int health, string[] entrancePoints, float speed, float delay)
			{
				this.active = active;
				this.health = health;
				this.entrancePoints = entrancePoints;
				this.speed = speed;
				this.delay = delay;
			}
		}

		public class BlackHole : AbstractLevelPropertyGroup
		{
			public readonly string[] patterns;

			public readonly float chargeTime;

			public readonly float attackTime;

			public readonly float speed;

			public readonly float health;

			public readonly float hesitate;

			public readonly float childDelay;

			public readonly int childSpeed;

			public readonly float childHealth;

			public readonly bool damageable;

			public BlackHole(string[] patterns, float chargeTime, float attackTime, float speed, float health, float hesitate, float childDelay, int childSpeed, float childHealth, bool damageable)
			{
				this.patterns = patterns;
				this.chargeTime = chargeTime;
				this.attackTime = attackTime;
				this.speed = speed;
				this.health = health;
				this.hesitate = hesitate;
				this.childDelay = childDelay;
				this.childSpeed = childSpeed;
				this.childHealth = childHealth;
				this.damageable = damageable;
			}
		}

		public class Triangle : AbstractLevelPropertyGroup
		{
			public readonly int count;

			public readonly float chargeTime;

			public readonly float attackTime;

			public readonly float introTime;

			public readonly float speed;

			public readonly float rotationSpeed;

			public readonly float health;

			public readonly float hesitate;

			public readonly float childSpeed;

			public readonly float childDelay;

			public readonly float childHealth;

			public readonly int childCount;

			public readonly bool damageable;

			public Triangle(int count, float chargeTime, float attackTime, float introTime, float speed, float rotationSpeed, float health, float hesitate, float childSpeed, float childDelay, float childHealth, int childCount, bool damageable)
			{
				this.count = count;
				this.chargeTime = chargeTime;
				this.attackTime = attackTime;
				this.introTime = introTime;
				this.speed = speed;
				this.rotationSpeed = rotationSpeed;
				this.health = health;
				this.hesitate = hesitate;
				this.childSpeed = childSpeed;
				this.childDelay = childDelay;
				this.childHealth = childHealth;
				this.childCount = childCount;
				this.damageable = damageable;
			}
		}

		public class Follower : AbstractLevelPropertyGroup
		{
			public readonly int count;

			public readonly float chargeTime;

			public readonly float attackTime;

			public readonly float introTime;

			public readonly float homingSpeed;

			public readonly float homingRotation;

			public readonly float homingTime;

			public readonly float health;

			public readonly float hesitate;

			public readonly float childDelay;

			public readonly float childHealth;

			public readonly bool damageable;

			public readonly bool parryable;

			public Follower(int count, float chargeTime, float attackTime, float introTime, float homingSpeed, float homingRotation, float homingTime, float health, float hesitate, float childDelay, float childHealth, bool damageable, bool parryable)
			{
				this.count = count;
				this.chargeTime = chargeTime;
				this.attackTime = attackTime;
				this.introTime = introTime;
				this.homingSpeed = homingSpeed;
				this.homingRotation = homingRotation;
				this.homingTime = homingTime;
				this.health = health;
				this.hesitate = hesitate;
				this.childDelay = childDelay;
				this.childHealth = childHealth;
				this.damageable = damageable;
				this.parryable = parryable;
			}
		}

		public class Chain : AbstractLevelPropertyGroup
		{
			public readonly int count;

			public readonly float delay;

			public readonly float timeX;

			public readonly float timeY;

			public readonly float speed;

			public readonly float hesitate;

			public readonly bool chainForever;

			public Chain(int count, float delay, float timeX, float timeY, float speed, float hesitate, bool chainForever)
			{
				this.count = count;
				this.delay = delay;
				this.timeX = timeX;
				this.timeY = timeY;
				this.speed = speed;
				this.hesitate = hesitate;
				this.chainForever = chainForever;
			}
		}

		public class SecurityGuard : AbstractLevelPropertyGroup
		{
			public readonly float speed;

			public readonly MinMax attackDelay;

			public readonly float idleTime;

			public readonly float warningTime;

			public readonly float childSpeed;

			public readonly int childCount;

			public SecurityGuard(float speed, MinMax attackDelay, float idleTime, float warningTime, float childSpeed, int childCount)
			{
				this.speed = speed;
				this.attackDelay = attackDelay;
				this.idleTime = idleTime;
				this.warningTime = warningTime;
				this.childSpeed = childSpeed;
				this.childCount = childCount;
			}
		}

		public class General : AbstractLevelPropertyGroup
		{
			public readonly float screenScrollSpeed;

			public readonly float movementSpeed;

			public readonly float movementOffset;

			public General(float screenScrollSpeed, float movementSpeed, float movementOffset)
			{
				this.screenScrollSpeed = screenScrollSpeed;
				this.movementSpeed = movementSpeed;
				this.movementOffset = movementOffset;
			}
		}

		public class WingSwipe : AbstractLevelPropertyGroup
		{
			public readonly float movementSpeed;

			public readonly string[] attackCount;

			public readonly float attackDuration;

			public readonly float maxDistance;

			public readonly float warningDuration;

			public readonly float warningMovementSpeed;

			public readonly float warningMaxDistance;

			public readonly MinMax hesitateRange;

			public WingSwipe(float movementSpeed, string[] attackCount, float attackDuration, float maxDistance, float warningDuration, float warningMovementSpeed, float warningMaxDistance, MinMax hesitateRange)
			{
				this.movementSpeed = movementSpeed;
				this.attackCount = attackCount;
				this.attackDuration = attackDuration;
				this.maxDistance = maxDistance;
				this.warningDuration = warningDuration;
				this.warningMovementSpeed = warningMovementSpeed;
				this.warningMaxDistance = warningMaxDistance;
				this.hesitateRange = hesitateRange;
			}
		}

		public class TurbineBlasters : AbstractLevelPropertyGroup
		{
			public readonly float bulletSpeed;

			public readonly float bulletCircleTime;

			public readonly string[] attackDirectionString;

			public readonly float repeatDealy;

			public readonly MinMax hesitateRange;

			public TurbineBlasters(float bulletSpeed, float bulletCircleTime, string[] attackDirectionString, float repeatDealy, MinMax hesitateRange)
			{
				this.bulletSpeed = bulletSpeed;
				this.bulletCircleTime = bulletCircleTime;
				this.attackDirectionString = attackDirectionString;
				this.repeatDealy = repeatDealy;
				this.hesitateRange = hesitateRange;
			}
		}

		[CompilerGenerated]
		private static Dictionary<string, int> _003C_003Ef__switch_0024map6;

		public Bee(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1000f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.75f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.45f));
				break;
			case Level.Mode.Normal:
				timeline.health = 1200f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.78f));
				timeline.events.Add(new Level.Timeline.Event("Airplane", 0.42f));
				break;
			case Level.Mode.Hard:
				timeline.health = 1400f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.76f));
				timeline.events.Add(new Level.Timeline.Event("Airplane", 0.41f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null)
			{
				if (_003C_003Ef__switch_0024map6 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(7);
					dictionary.Add("B", 0);
					dictionary.Add("C", 1);
					dictionary.Add("T", 2);
					dictionary.Add("F", 3);
					dictionary.Add("S", 4);
					dictionary.Add("W", 5);
					dictionary.Add("U", 6);
					_003C_003Ef__switch_0024map6 = dictionary;
				}
				int value;
				if (_003C_003Ef__switch_0024map6.TryGetValue(id, out value))
				{
					switch (value)
					{
					case 0:
						return Pattern.BlackHole;
					case 1:
						return Pattern.Chain;
					case 2:
						return Pattern.Triangle;
					case 3:
						return Pattern.Follower;
					case 4:
						return Pattern.SecurityGuard;
					case 5:
						return Pattern.Wing;
					case 6:
						return Pattern.Turbine;
					}
				}
			}
			Debug.LogError("Pattern Bee.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static Bee GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] { Pattern.SecurityGuard } }, States.Main, new Movement(true, 25f, 0), new Grunts(true, 8, new string[1] { "0,1,2,3,2,1" }, 290f, 4f), new BlackHole(new string[2] { "2,0", "0,2" }, 3f, 1.5f, 450f, 1000000f, 3f, 0.1f, 500, 1f, true), new Triangle(1, 2.7f, 1.5f, 1f, 160f, 50f, 100000f, 3.5f, 480f, 1.8f, 1f, 2, false), new Follower(1, 2.7f, 1.5f, 1f, 270f, 1.4f, 3f, 10000f, 3.5f, 0f, 0f, false, true), new Chain(100000, 2.2f, 0.74f, 0.06f, 560f, 1f, true), new SecurityGuard(330f, new MinMax(1.5f, 3.3f), 1f, 2f, 360f, 8), new General(25f, 100f, 500f), new WingSwipe(0f, new string[0], 0f, 0f, 0f, 0f, 0f, new MinMax(0f, 0f)), new TurbineBlasters(0f, 0f, new string[0], 0f, new MinMax(0f, 0f))));
				list.Add(new State(0.75f, new Pattern[1][] { new Pattern[11]
				{
					Pattern.Follower,
					Pattern.Triangle,
					Pattern.Follower,
					Pattern.Triangle,
					Pattern.Triangle,
					Pattern.Follower,
					Pattern.Triangle,
					Pattern.Follower,
					Pattern.Triangle,
					Pattern.Follower,
					Pattern.Follower
				} }, States.Generic, new Movement(true, 50f, 0), new Grunts(false, 1, new string[1] { "1,2,3,0" }, 300f, 5f), new BlackHole(new string[2] { "2,0", "0,2" }, 3f, 1.5f, 450f, 1000000f, 3f, 0.1f, 500, 1f, true), new Triangle(1, 2.7f, 1.5f, 1f, 160f, 50f, 100000f, 3.5f, 480f, 1.8f, 1f, 2, false), new Follower(1, 2.7f, 1.5f, 1f, 270f, 1.4f, 3f, 10000f, 3.5f, 0f, 0f, false, true), new Chain(100000, 2.2f, 0.74f, 0.06f, 560f, 1f, true), new SecurityGuard(330f, new MinMax(1.5f, 3.3f), 1f, 2f, 360f, 8), new General(25f, 100f, 500f), new WingSwipe(0f, new string[0], 0f, 0f, 0f, 0f, 0f, new MinMax(0f, 0f)), new TurbineBlasters(0f, 0f, new string[0], 0f, new MinMax(0f, 0f))));
				list.Add(new State(0.45f, new Pattern[1][] { new Pattern[1] { Pattern.Chain } }, States.Generic, new Movement(true, 200f, 0), new Grunts(false, 1, new string[1] { "0,1,2,3" }, 400f, 9999f), new BlackHole(new string[2] { "2,0", "0,2" }, 3f, 1.5f, 450f, 1000000f, 3f, 0.1f, 500, 1f, true), new Triangle(1, 2.7f, 1.5f, 1f, 160f, 50f, 100000f, 3.5f, 480f, 1.8f, 1f, 2, false), new Follower(1, 2.7f, 1.5f, 1f, 270f, 1.4f, 3f, 10000f, 3.5f, 0f, 0f, false, true), new Chain(100000, 2.2f, 0.74f, 0.06f, 560f, 1f, true), new SecurityGuard(330f, new MinMax(1.5f, 3.3f), 1f, 2f, 360f, 8), new General(25f, 100f, 500f), new WingSwipe(0f, new string[0], 0f, 0f, 0f, 0f, 0f, new MinMax(0f, 0f)), new TurbineBlasters(0f, 0f, new string[0], 0f, new MinMax(0f, 0f))));
				break;
			case Level.Mode.Normal:
				hp = 1200;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] { Pattern.SecurityGuard } }, States.Main, new Movement(true, 100f, 1), new Grunts(true, 8, new string[1] { "0,1,2,3,2,1" }, 320f, 4f), new BlackHole(new string[2] { "2,0", "0,2" }, 4f, 1f, 200f, 10000f, 3f, 0.8f, 550, 1f, true), new Triangle(2, 2.7f, 1.5f, 1f, 160f, 50f, 10000f, 3.5f, 550f, 1.4f, 1f, 3, false), new Follower(2, 2.7f, 1.5f, 1f, 250f, 1.5f, 2.6f, 10000f, 3f, 1f, 2f, false, true), new Chain(6, 1.5f, 0.6f, 0.06f, 620f, 1f, false), new SecurityGuard(300f, new MinMax(1.8f, 3f), 1f, 2f, 450f, 6), new General(150f, 100f, 500f), new WingSwipe(435f, new string[1] { "1,2,1,1,2,1,1,2,1,1,1,2,1,1,2" }, 0.5f, 300f, 0.9f, 345f, 400f, new MinMax(1.5f, 3.5f)), new TurbineBlasters(500f, 1.5f, new string[6] { "L,R,L", "R,L,R,L", "L,R", "L,R,L,R", "R,L,R", "R,L" }, 3.3f, new MinMax(1f, 2f))));
				list.Add(new State(0.78f, new Pattern[1][] { new Pattern[3]
				{
					Pattern.Follower,
					Pattern.Triangle,
					Pattern.Chain
				} }, States.Generic, new Movement(true, 100f, 1), new Grunts(false, 2, new string[1] { "0,1,2,3,2,1" }, 320f, 10000f), new BlackHole(new string[2] { "2,0", "0,2" }, 4f, 1f, 200f, 10000f, 3f, 0.8f, 550, 1f, true), new Triangle(2, 2.7f, 1.5f, 1f, 160f, 50f, 10000f, 3.5f, 550f, 1.4f, 1f, 3, false), new Follower(2, 2.7f, 1.5f, 1f, 250f, 1.5f, 2.6f, 10000f, 3f, 1f, 2f, false, true), new Chain(6, 1.5f, 0.6f, 0.06f, 620f, 1f, false), new SecurityGuard(300f, new MinMax(1.8f, 3f), 1f, 2f, 450f, 6), new General(150f, 100f, 500f), new WingSwipe(435f, new string[1] { "1,2,1,1,2,1,1,2,1,1,1,2,1,1,2" }, 0.5f, 300f, 0.9f, 345f, 400f, new MinMax(1.5f, 3.5f)), new TurbineBlasters(500f, 1.5f, new string[6] { "L,R,L", "R,L,R,L", "L,R", "L,R,L,R", "R,L,R", "R,L" }, 3.3f, new MinMax(1f, 2f))));
				list.Add(new State(0.42f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Wing,
					Pattern.Turbine
				} }, States.Airplane, new Movement(true, 100f, 1), new Grunts(false, 2, new string[1] { "0,1,2,3,2,1" }, 320f, 10000f), new BlackHole(new string[2] { "2,0", "0,2" }, 4f, 1f, 200f, 10000f, 3f, 0.8f, 550, 1f, true), new Triangle(2, 2.7f, 1.5f, 1f, 160f, 50f, 10000f, 3.5f, 550f, 1.4f, 1f, 3, false), new Follower(2, 2.7f, 1.5f, 1f, 250f, 1.5f, 2.6f, 10000f, 3f, 1f, 2f, false, true), new Chain(6, 1.5f, 0.6f, 0.06f, 620f, 1f, false), new SecurityGuard(300f, new MinMax(1.8f, 3f), 1f, 2f, 450f, 6), new General(150f, 100f, 500f), new WingSwipe(435f, new string[1] { "1,2,1,1,2,1,1,2,1,1,1,2,1,1,2" }, 0.5f, 300f, 0.9f, 345f, 400f, new MinMax(1.5f, 3.5f)), new TurbineBlasters(500f, 1.5f, new string[6] { "L,R,L", "R,L,R,L", "L,R", "L,R,L,R", "R,L,R", "R,L" }, 3.3f, new MinMax(1f, 2f))));
				break;
			case Level.Mode.Hard:
				hp = 1400;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] { Pattern.SecurityGuard } }, States.Main, new Movement(true, 135f, 1), new Grunts(true, 8, new string[1] { "0,1,2,3,2,1" }, 320f, 3.5f), new BlackHole(new string[2] { "2,0", "0,2" }, 4f, 1f, 200f, 1000000f, 3f, 0.8f, 550, 1f, true), new Triangle(1, 2f, 1.5f, 1f, 180f, 60f, 100000f, 2.5f, 550f, 1f, 1f, 3, false), new Follower(1, 2f, 1.5f, 1f, 330f, 2.5f, 2.5f, 10000f, 2.5f, 0f, 0f, false, true), new Chain(4, 1.4f, 0.55f, 0.05f, 700f, 1f, false), new SecurityGuard(325f, new MinMax(1.4f, 2.7f), 1f, 2f, 450f, 8), new General(180f, 120f, 500f), new WingSwipe(480f, new string[1] { "1,2,1,1,2,1,1,2,1,1,1,2" }, 0.1f, 225f, 0.9f, 370f, 400f, new MinMax(1.5f, 3.5f)), new TurbineBlasters(550f, 1.6f, new string[4] { "L,R,D:0.1,B", "R,L,R", "R,L,D:0.1,B", "L,R,L" }, 3.1f, new MinMax(1f, 2f))));
				list.Add(new State(0.76f, new Pattern[1][] { new Pattern[4]
				{
					Pattern.Follower,
					Pattern.Chain,
					Pattern.Triangle,
					Pattern.Chain
				} }, States.Generic, new Movement(true, 135f, 1), new Grunts(false, 8, new string[1] { "0,1,2,3" }, 300f, 40000f), new BlackHole(new string[2] { "2,0", "0,2" }, 4f, 1f, 200f, 1000000f, 3f, 0.8f, 550, 1f, true), new Triangle(1, 2f, 1.5f, 1f, 180f, 60f, 100000f, 2.5f, 550f, 1f, 1f, 3, false), new Follower(1, 2f, 1.5f, 1f, 330f, 2.5f, 2.5f, 10000f, 2.5f, 0f, 0f, false, true), new Chain(4, 1.4f, 0.55f, 0.05f, 700f, 1f, false), new SecurityGuard(325f, new MinMax(1.4f, 2.7f), 1f, 2f, 450f, 8), new General(180f, 120f, 500f), new WingSwipe(480f, new string[1] { "1,2,1,1,2,1,1,2,1,1,1,2" }, 0.1f, 225f, 0.9f, 370f, 400f, new MinMax(1.5f, 3.5f)), new TurbineBlasters(550f, 1.6f, new string[4] { "L,R,D:0.1,B", "R,L,R", "R,L,D:0.1,B", "L,R,L" }, 3.1f, new MinMax(1f, 2f))));
				list.Add(new State(0.41f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Wing,
					Pattern.Turbine
				} }, States.Airplane, new Movement(true, 135f, 1), new Grunts(false, 8, new string[1] { "0,1,2,3" }, 300f, 40000f), new BlackHole(new string[2] { "2,0", "0,2" }, 4f, 1f, 200f, 1000000f, 3f, 0.8f, 550, 1f, true), new Triangle(1, 2f, 1.5f, 1f, 180f, 60f, 100000f, 2.5f, 550f, 1f, 1f, 3, false), new Follower(1, 2f, 1.5f, 1f, 330f, 2.5f, 2.5f, 10000f, 2.5f, 0f, 0f, false, true), new Chain(4, 1.4f, 0.55f, 0.05f, 700f, 1f, false), new SecurityGuard(325f, new MinMax(1.4f, 2.7f), 1f, 2f, 450f, 8), new General(180f, 120f, 500f), new WingSwipe(480f, new string[1] { "1,2,1,1,2,1,1,2,1,1,1,2" }, 0.1f, 225f, 0.9f, 370f, 400f, new MinMax(1.5f, 3.5f)), new TurbineBlasters(550f, 1.6f, new string[4] { "L,R,D:0.1,B", "R,L,R", "R,L,D:0.1,B", "L,R,L" }, 3.1f, new MinMax(1f, 2f))));
				break;
			}
			return new Bee(hp, goalTimes, list.ToArray());
		}
	}

	public class Clown : AbstractLevelProperties<Clown.State, Clown.Pattern, Clown.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Clown properties { get; private set; }

			public virtual void LevelInit(Clown properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			HeliumTank = 2,
			CarouselHorse = 3,
			Swing = 4
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly BumperCar bumperCar;

			public readonly Duck duck;

			public readonly HeliumClown heliumClown;

			public readonly Horse horse;

			public readonly Coaster coaster;

			public readonly Swing swing;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, BumperCar bumperCar, Duck duck, HeliumClown heliumClown, Horse horse, Coaster coaster, Swing swing)
				: base(healthTrigger, patterns, stateName)
			{
				this.bumperCar = bumperCar;
				this.duck = duck;
				this.heliumClown = heliumClown;
				this.horse = horse;
				this.coaster = coaster;
				this.swing = swing;
			}
		}

		public class BumperCar : AbstractLevelPropertyGroup
		{
			public readonly float movementSpeed;

			public readonly float dashSpeed;

			public readonly string[] attackDelayString;

			public readonly float movementDuration;

			public readonly string[] movementStrings;

			public readonly float bumperDashWarning;

			public readonly float movementDelay;

			public BumperCar(float movementSpeed, float dashSpeed, string[] attackDelayString, float movementDuration, string[] movementStrings, float bumperDashWarning, float movementDelay)
			{
				this.movementSpeed = movementSpeed;
				this.dashSpeed = dashSpeed;
				this.attackDelayString = attackDelayString;
				this.movementDuration = movementDuration;
				this.movementStrings = movementStrings;
				this.bumperDashWarning = bumperDashWarning;
				this.movementDelay = movementDelay;
			}
		}

		public class Duck : AbstractLevelPropertyGroup
		{
			public readonly MinMax duckYHeightRange;

			public readonly string[] duckYStartPercentString;

			public readonly float duckXMovementSpeed;

			public readonly float duckYMovementSpeed;

			public readonly string[] duckTypeString;

			public readonly float duckDelay;

			public readonly float spinDuration;

			public readonly float bombSpeed;

			public Duck(MinMax duckYHeightRange, string[] duckYStartPercentString, float duckXMovementSpeed, float duckYMovementSpeed, string[] duckTypeString, float duckDelay, float spinDuration, float bombSpeed)
			{
				this.duckYHeightRange = duckYHeightRange;
				this.duckYStartPercentString = duckYStartPercentString;
				this.duckXMovementSpeed = duckXMovementSpeed;
				this.duckYMovementSpeed = duckYMovementSpeed;
				this.duckTypeString = duckTypeString;
				this.duckDelay = duckDelay;
				this.spinDuration = spinDuration;
				this.bombSpeed = bombSpeed;
			}
		}

		public class HeliumClown : AbstractLevelPropertyGroup
		{
			public readonly bool coasterOn;

			public readonly float dogHP;

			public readonly float dogSpeed;

			public readonly string[] dogDelayString;

			public readonly string[] dogTypeString;

			public readonly string[] dogSpawnOrder;

			public readonly float heliumMoveSpeed;

			public readonly float heliumAcceleration;

			public readonly bool dogDieOnGround;

			public HeliumClown(bool coasterOn, float dogHP, float dogSpeed, string[] dogDelayString, string[] dogTypeString, string[] dogSpawnOrder, float heliumMoveSpeed, float heliumAcceleration, bool dogDieOnGround)
			{
				this.coasterOn = coasterOn;
				this.dogHP = dogHP;
				this.dogSpeed = dogSpeed;
				this.dogDelayString = dogDelayString;
				this.dogTypeString = dogTypeString;
				this.dogSpawnOrder = dogSpawnOrder;
				this.heliumMoveSpeed = heliumMoveSpeed;
				this.heliumAcceleration = heliumAcceleration;
				this.dogDieOnGround = dogDieOnGround;
			}
		}

		public class Horse : AbstractLevelPropertyGroup
		{
			public readonly bool coasterOn;

			public readonly float HorseSpeed;

			public readonly string[] HorseString;

			public readonly float HorseXPosOffset;

			public readonly int WaveBulletCount;

			public readonly float WaveBulletSpeed;

			public readonly float WaveBulletAmount;

			public readonly float WaveBulletWaveSpeed;

			public readonly float WaveBulletDelay;

			public readonly float WaveATKDelay;

			public readonly float WaveATKRepeat;

			public readonly string[] WavePosString;

			public readonly string[] WavePinkString;

			public readonly float WaveHesitate;

			public readonly float DropBulletInitalSpeed;

			public readonly float DropBulletDelay;

			public readonly MinMax DropBulletOneDelay;

			public readonly MinMax DropBulletTwoDelay;

			public readonly float DropBulletSpeedDown;

			public readonly float DropATKDelay;

			public readonly string[] DropHorsePositionString;

			public readonly string[] DropBulletPositionString;

			public readonly float DropHesitate;

			public readonly float DropATKRepeat;

			public readonly float DropBulletReturnDelay;

			public Horse(bool coasterOn, float HorseSpeed, string[] HorseString, float HorseXPosOffset, int WaveBulletCount, float WaveBulletSpeed, float WaveBulletAmount, float WaveBulletWaveSpeed, float WaveBulletDelay, float WaveATKDelay, float WaveATKRepeat, string[] WavePosString, string[] WavePinkString, float WaveHesitate, float DropBulletInitalSpeed, float DropBulletDelay, MinMax DropBulletOneDelay, MinMax DropBulletTwoDelay, float DropBulletSpeedDown, float DropATKDelay, string[] DropHorsePositionString, string[] DropBulletPositionString, float DropHesitate, float DropATKRepeat, float DropBulletReturnDelay)
			{
				this.coasterOn = coasterOn;
				this.HorseSpeed = HorseSpeed;
				this.HorseString = HorseString;
				this.HorseXPosOffset = HorseXPosOffset;
				this.WaveBulletCount = WaveBulletCount;
				this.WaveBulletSpeed = WaveBulletSpeed;
				this.WaveBulletAmount = WaveBulletAmount;
				this.WaveBulletWaveSpeed = WaveBulletWaveSpeed;
				this.WaveBulletDelay = WaveBulletDelay;
				this.WaveATKDelay = WaveATKDelay;
				this.WaveATKRepeat = WaveATKRepeat;
				this.WavePosString = WavePosString;
				this.WavePinkString = WavePinkString;
				this.WaveHesitate = WaveHesitate;
				this.DropBulletInitalSpeed = DropBulletInitalSpeed;
				this.DropBulletDelay = DropBulletDelay;
				this.DropBulletOneDelay = DropBulletOneDelay;
				this.DropBulletTwoDelay = DropBulletTwoDelay;
				this.DropBulletSpeedDown = DropBulletSpeedDown;
				this.DropATKDelay = DropATKDelay;
				this.DropHorsePositionString = DropHorsePositionString;
				this.DropBulletPositionString = DropBulletPositionString;
				this.DropHesitate = DropHesitate;
				this.DropATKRepeat = DropATKRepeat;
				this.DropBulletReturnDelay = DropBulletReturnDelay;
			}
		}

		public class Coaster : AbstractLevelPropertyGroup
		{
			public readonly MinMax initialDelay;

			public readonly float noseParrySuperGain;

			public readonly float coasterSpeed;

			public readonly float coasterBackSpeedMultipler;

			public readonly MinMax mainLoopDelay;

			public readonly string[] coasterTypeString;

			public readonly float coasterBackToFrontDelay;

			public Coaster(MinMax initialDelay, float noseParrySuperGain, float coasterSpeed, float coasterBackSpeedMultipler, MinMax mainLoopDelay, string[] coasterTypeString, float coasterBackToFrontDelay)
			{
				this.initialDelay = initialDelay;
				this.noseParrySuperGain = noseParrySuperGain;
				this.coasterSpeed = coasterSpeed;
				this.coasterBackSpeedMultipler = coasterBackSpeedMultipler;
				this.mainLoopDelay = mainLoopDelay;
				this.coasterTypeString = coasterTypeString;
				this.coasterBackToFrontDelay = coasterBackToFrontDelay;
			}
		}

		public class Swing : AbstractLevelPropertyGroup
		{
			public readonly float swingSpeed;

			public readonly float swingSpacing;

			public readonly float swingDropWarningDuration;

			public readonly float swingfullDropDuration;

			public readonly bool swingDropOn;

			public readonly float bulletSpeed;

			public readonly MinMax attackDelayRange;

			public readonly float HP;

			public readonly float movementSpeed;

			public readonly string[] positionString;

			public readonly float spawnDelay;

			public readonly float initialAttackDelay;

			public Swing(float swingSpeed, float swingSpacing, float swingDropWarningDuration, float swingfullDropDuration, bool swingDropOn, float bulletSpeed, MinMax attackDelayRange, float HP, float movementSpeed, string[] positionString, float spawnDelay, float initialAttackDelay)
			{
				this.swingSpeed = swingSpeed;
				this.swingSpacing = swingSpacing;
				this.swingDropWarningDuration = swingDropWarningDuration;
				this.swingfullDropDuration = swingfullDropDuration;
				this.swingDropOn = swingDropOn;
				this.bulletSpeed = bulletSpeed;
				this.attackDelayRange = attackDelayRange;
				this.HP = HP;
				this.movementSpeed = movementSpeed;
				this.positionString = positionString;
				this.spawnDelay = spawnDelay;
				this.initialAttackDelay = initialAttackDelay;
			}
		}

		public Clown(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1200f;
				timeline.events.Add(new Level.Timeline.Event("HeliumTank", 0.7f));
				timeline.events.Add(new Level.Timeline.Event("CarouselHorse", 0.35f));
				break;
			case Level.Mode.Normal:
				timeline.health = 1550f;
				timeline.events.Add(new Level.Timeline.Event("HeliumTank", 0.83f));
				timeline.events.Add(new Level.Timeline.Event("CarouselHorse", 0.63f));
				timeline.events.Add(new Level.Timeline.Event("Swing", 0.45f));
				break;
			case Level.Mode.Hard:
				timeline.health = 1850f;
				timeline.events.Add(new Level.Timeline.Event("HeliumTank", 0.83f));
				timeline.events.Add(new Level.Timeline.Event("CarouselHorse", 0.63f));
				timeline.events.Add(new Level.Timeline.Event("Swing", 0.45f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern Clown.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static Clown GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1200;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new BumperCar(475f, 1200f, new string[1] { "3.3,1.8,3.5,3,1.5,2.7,3,2,1.5" }, 0.6f, new string[1] { "F,F,B,B,F,B,F,B,B,F,B,F,F,B,F,B" }, 1f, 0.3f), new Duck(new MinMax(90f, 140f), new string[1] { "0,10,20,30,40,50" }, 210f, 420f, new string[1] { "R,R,P,R,R,R,R,P" }, 3f, 10f, 350f), new HeliumClown(false, 10f, 430f, new string[1] { "1,1.3,1,1.5,0.8,0.9,1.6,1.8,1.5,1,1.7,1.8" }, new string[1] { "R,R,R,R,P,R,R,R,R,R,P" }, new string[6] { "6,4,3,1,5,2", "1,3,4,6,2,5", "1,2,3,4,5,6", "1,3,2,5,3,4", "3,4,5,2,6,1", "6,5,4,3,2,1" }, 200f, 15f, true), new Horse(false, 450f, new string[1] { "W,D,D,W,D,W,W,D" }, 200f, 1, 350f, 30f, 4.6f, 0.3f, 1.4f, 3f, new string[1] { "205" }, new string[5] { "P", "R", "R", "P", "R" }, 1f, 1200f, 1f, new MinMax(0.1f, 0.2f), new MinMax(0.1f, 0.2f), 800f, 2f, new string[1] { "235" }, new string[8] { "400-500-800-900-1000-1100-1200-1300", "400-500-600-700-800-900-1200-1300", "400-700-800-900-1000-1100-1200-1300", "400-500-600-700-1000-1100-1200-1300", "400-500-600-900-1000-1100-1200-1300", "400-500-600-700-800-900-1000-1300", "400-500-800-900-1000-1100-1200-1300", "400-500-600-700-800-1100-1200-1300" }, 0.5f, 1f, 0.3f), new Coaster(new MinMax(0f, 1f), 0f, 0f, 0f, new MinMax(0f, 1f), new string[0], 0f), new Swing(0f, 0f, 0f, 0f, false, 0f, new MinMax(0f, 1f), 0f, 0f, new string[0], 0f, 0f)));
				list.Add(new State(0.7f, new Pattern[1][] { new Pattern[0] }, States.HeliumTank, new BumperCar(475f, 1200f, new string[1] { "3.3,1.8,3.5,3,1.5,2.7,3,2,1.5" }, 0.6f, new string[1] { "F,F,B,B,F,B,F,B,B,F,B,F,F,B,F,B" }, 1f, 0.3f), new Duck(new MinMax(90f, 140f), new string[1] { "0,10,20,30,40,50" }, 210f, 420f, new string[1] { "R,R,P,R,R,R,R,P" }, 3f, 10f, 350f), new HeliumClown(false, 10f, 430f, new string[1] { "1,1.3,1,1.5,0.8,0.9,1.6,1.8,1.5,1,1.7,1.8" }, new string[1] { "R,R,R,R,P,R,R,R,R,R,P" }, new string[6] { "6,4,3,1,5,2", "1,3,4,6,2,5", "1,2,3,4,5,6", "1,3,2,5,3,4", "3,4,5,2,6,1", "6,5,4,3,2,1" }, 200f, 15f, true), new Horse(false, 450f, new string[1] { "W,D,D,W,D,W,W,D" }, 200f, 1, 350f, 30f, 4.6f, 0.3f, 1.4f, 3f, new string[1] { "205" }, new string[5] { "P", "R", "R", "P", "R" }, 1f, 1200f, 1f, new MinMax(0.1f, 0.2f), new MinMax(0.1f, 0.2f), 800f, 2f, new string[1] { "235" }, new string[8] { "400-500-800-900-1000-1100-1200-1300", "400-500-600-700-800-900-1200-1300", "400-700-800-900-1000-1100-1200-1300", "400-500-600-700-1000-1100-1200-1300", "400-500-600-900-1000-1100-1200-1300", "400-500-600-700-800-900-1000-1300", "400-500-800-900-1000-1100-1200-1300", "400-500-600-700-800-1100-1200-1300" }, 0.5f, 1f, 0.3f), new Coaster(new MinMax(0f, 1f), 0f, 0f, 0f, new MinMax(0f, 1f), new string[0], 0f), new Swing(0f, 0f, 0f, 0f, false, 0f, new MinMax(0f, 1f), 0f, 0f, new string[0], 0f, 0f)));
				list.Add(new State(0.35f, new Pattern[1][] { new Pattern[0] }, States.CarouselHorse, new BumperCar(475f, 1200f, new string[1] { "3.3,1.8,3.5,3,1.5,2.7,3,2,1.5" }, 0.6f, new string[1] { "F,F,B,B,F,B,F,B,B,F,B,F,F,B,F,B" }, 1f, 0.3f), new Duck(new MinMax(90f, 140f), new string[1] { "0,10,20,30,40,50" }, 210f, 420f, new string[1] { "R,R,P,R,R,R,R,P" }, 3f, 10f, 350f), new HeliumClown(false, 10f, 430f, new string[1] { "1,1.3,1,1.5,0.8,0.9,1.6,1.8,1.5,1,1.7,1.8" }, new string[1] { "R,R,R,R,P,R,R,R,R,R,P" }, new string[6] { "6,4,3,1,5,2", "1,3,4,6,2,5", "1,2,3,4,5,6", "1,3,2,5,3,4", "3,4,5,2,6,1", "6,5,4,3,2,1" }, 200f, 15f, true), new Horse(false, 450f, new string[1] { "W,D,D,W,D,W,W,D" }, 200f, 1, 350f, 30f, 4.6f, 0.3f, 1.4f, 3f, new string[1] { "205" }, new string[5] { "P", "R", "R", "P", "R" }, 1f, 1200f, 1f, new MinMax(0.1f, 0.2f), new MinMax(0.1f, 0.2f), 800f, 2f, new string[1] { "235" }, new string[8] { "400-500-800-900-1000-1100-1200-1300", "400-500-600-700-800-900-1200-1300", "400-700-800-900-1000-1100-1200-1300", "400-500-600-700-1000-1100-1200-1300", "400-500-600-900-1000-1100-1200-1300", "400-500-600-700-800-900-1000-1300", "400-500-800-900-1000-1100-1200-1300", "400-500-600-700-800-1100-1200-1300" }, 0.5f, 1f, 0.3f), new Coaster(new MinMax(0f, 1f), 0f, 0f, 0f, new MinMax(0f, 1f), new string[0], 0f), new Swing(0f, 0f, 0f, 0f, false, 0f, new MinMax(0f, 1f), 0f, 0f, new string[0], 0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 1550;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new BumperCar(500f, 1300f, new string[1] { "3.3,2.4,3.5,3,1.5,3.5,3,3.8,2,1.5" }, 0.6f, new string[1] { "F,F,B,B,F,B,F,B,B,F,B,F,F,B,F,B" }, 1f, 0.3f), new Duck(new MinMax(110f, 160f), new string[1] { "0,10,20,30,40,50" }, 210f, 420f, new string[2] { "R,B,R,R,R,B,R,P", "R,R,B,P,R,R,B,R" }, 2.8f, 12f, 350f), new HeliumClown(true, 5.3f, 300f, new string[1] { "0.7,1,0.9,2,1.4,0.8,1.3,1.5,0.9,1.4,1.2" }, new string[1] { "R,R,R,R,P,R,R,R,R,R,P" }, new string[2] { "1,3,5,6,4,3,1,6,2,4,1,5,6,2,1,3,6,1,4,2,6,4,3,5,2", "1,2,4,3,5,6,3,4,1,3,2,5,6,1,3,4,2,1,3,5,2,6" }, 200f, 15f, true), new Horse(true, 375f, new string[1] { "W,D,D,W,D,W,W,D" }, 200f, 2, 320f, 30f, 4f, 0.3f, 3f, 2f, new string[2] { "105,115,110", "100,110,105" }, new string[4] { "P,R", "R,R", "R,P", "R,R" }, 1f, 1600f, 1.1f, new MinMax(0.1f, 0.2f), new MinMax(0.1f, 0.2f), 1000f, 2.5f, new string[1] { "75" }, new string[9] { "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-1200-1300", "400-500-600-1000-1100-1200-1300", "400-500-600-700-800-900-1300", "400-500-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300", "400-500-600-700-800-900-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300" }, 1f, 1f, 0.5f), new Coaster(new MinMax(4f, 6f), 1f, 425f, 1.25f, new MinMax(1f, 2f), new string[1] { "E,E,F,E,E,E,F,E,E" }, 1f), new Swing(300f, 200f, 0.5f, 0.7f, false, 500f, new MinMax(2.4f, 4.8f), 13f, 700f, new string[8] { "0,400,800,1200", "1150-775-375-50", "100-500-1100", "0,150,1050,1200", "50,500,700,1150", "1200,800,400,0", "1100-700-100", "0,200,1000,1200" }, 0.8f, 0.01f)));
				list.Add(new State(0.83f, new Pattern[1][] { new Pattern[0] }, States.HeliumTank, new BumperCar(500f, 1300f, new string[1] { "3.3,2.4,3.5,3,1.5,3.5,3,3.8,2,1.5" }, 0.6f, new string[1] { "F,F,B,B,F,B,F,B,B,F,B,F,F,B,F,B" }, 1f, 0.3f), new Duck(new MinMax(110f, 160f), new string[1] { "0,10,20,30,40,50" }, 210f, 420f, new string[2] { "R,B,R,R,R,B,R,P", "R,R,B,P,R,R,B,R" }, 2.8f, 12f, 350f), new HeliumClown(true, 5.3f, 300f, new string[1] { "0.7,1,0.9,2,1.4,0.8,1.3,1.5,0.9,1.4,1.2" }, new string[1] { "R,R,R,R,P,R,R,R,R,R,P" }, new string[2] { "1,3,5,6,4,3,1,6,2,4,1,5,6,2,1,3,6,1,4,2,6,4,3,5,2", "1,2,4,3,5,6,3,4,1,3,2,5,6,1,3,4,2,1,3,5,2,6" }, 200f, 15f, true), new Horse(true, 375f, new string[1] { "W,D,D,W,D,W,W,D" }, 200f, 2, 320f, 30f, 4f, 0.3f, 3f, 2f, new string[2] { "105,115,110", "100,110,105" }, new string[4] { "P,R", "R,R", "R,P", "R,R" }, 1f, 1600f, 1.1f, new MinMax(0.1f, 0.2f), new MinMax(0.1f, 0.2f), 1000f, 2.5f, new string[1] { "75" }, new string[9] { "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-1200-1300", "400-500-600-1000-1100-1200-1300", "400-500-600-700-800-900-1300", "400-500-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300", "400-500-600-700-800-900-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300" }, 1f, 1f, 0.5f), new Coaster(new MinMax(4f, 6f), 1f, 425f, 1.25f, new MinMax(1f, 2f), new string[1] { "E,E,F,E,E,E,F,E,E" }, 1f), new Swing(300f, 200f, 0.5f, 0.7f, false, 500f, new MinMax(2.4f, 4.8f), 13f, 700f, new string[8] { "0,400,800,1200", "1150-775-375-50", "100-500-1100", "0,150,1050,1200", "50,500,700,1150", "1200,800,400,0", "1100-700-100", "0,200,1000,1200" }, 0.8f, 0.01f)));
				list.Add(new State(0.63f, new Pattern[1][] { new Pattern[0] }, States.CarouselHorse, new BumperCar(500f, 1300f, new string[1] { "3.3,2.4,3.5,3,1.5,3.5,3,3.8,2,1.5" }, 0.6f, new string[1] { "F,F,B,B,F,B,F,B,B,F,B,F,F,B,F,B" }, 1f, 0.3f), new Duck(new MinMax(110f, 160f), new string[1] { "0,10,20,30,40,50" }, 210f, 420f, new string[2] { "R,B,R,R,R,B,R,P", "R,R,B,P,R,R,B,R" }, 2.8f, 12f, 350f), new HeliumClown(true, 5.3f, 300f, new string[1] { "0.7,1,0.9,2,1.4,0.8,1.3,1.5,0.9,1.4,1.2" }, new string[1] { "R,R,R,R,P,R,R,R,R,R,P" }, new string[2] { "1,3,5,6,4,3,1,6,2,4,1,5,6,2,1,3,6,1,4,2,6,4,3,5,2", "1,2,4,3,5,6,3,4,1,3,2,5,6,1,3,4,2,1,3,5,2,6" }, 200f, 15f, true), new Horse(true, 375f, new string[1] { "W,D,D,W,D,W,W,D" }, 200f, 2, 320f, 30f, 4f, 0.3f, 3f, 2f, new string[2] { "105,115,110", "100,110,105" }, new string[4] { "P,R", "R,R", "R,P", "R,R" }, 1f, 1600f, 1.1f, new MinMax(0.1f, 0.2f), new MinMax(0.1f, 0.2f), 1000f, 2.5f, new string[1] { "75" }, new string[9] { "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-1200-1300", "400-500-600-1000-1100-1200-1300", "400-500-600-700-800-900-1300", "400-500-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300", "400-500-600-700-800-900-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300" }, 1f, 1f, 0.5f), new Coaster(new MinMax(1.5f, 3f), 1f, 475f, 1.75f, new MinMax(1f, 2f), new string[1] { "E,E,F,E,E,E,F,E,E" }, 0.8f), new Swing(300f, 200f, 0.5f, 0.7f, false, 500f, new MinMax(2.4f, 4.8f), 13f, 700f, new string[8] { "0,400,800,1200", "1150-775-375-50", "100-500-1100", "0,150,1050,1200", "50,500,700,1150", "1200,800,400,0", "1100-700-100", "0,200,1000,1200" }, 0.8f, 0.01f)));
				list.Add(new State(0.45f, new Pattern[1][] { new Pattern[0] }, States.Swing, new BumperCar(500f, 1300f, new string[1] { "3.3,2.4,3.5,3,1.5,3.5,3,3.8,2,1.5" }, 0.6f, new string[1] { "F,F,B,B,F,B,F,B,B,F,B,F,F,B,F,B" }, 1f, 0.3f), new Duck(new MinMax(110f, 160f), new string[1] { "0,10,20,30,40,50" }, 210f, 420f, new string[2] { "R,B,R,R,R,B,R,P", "R,R,B,P,R,R,B,R" }, 2.8f, 12f, 350f), new HeliumClown(true, 5.3f, 300f, new string[1] { "0.7,1,0.9,2,1.4,0.8,1.3,1.5,0.9,1.4,1.2" }, new string[1] { "R,R,R,R,P,R,R,R,R,R,P" }, new string[2] { "1,3,5,6,4,3,1,6,2,4,1,5,6,2,1,3,6,1,4,2,6,4,3,5,2", "1,2,4,3,5,6,3,4,1,3,2,5,6,1,3,4,2,1,3,5,2,6" }, 200f, 15f, true), new Horse(true, 375f, new string[1] { "W,D,D,W,D,W,W,D" }, 200f, 2, 320f, 30f, 4f, 0.3f, 3f, 2f, new string[2] { "105,115,110", "100,110,105" }, new string[4] { "P,R", "R,R", "R,P", "R,R" }, 1f, 1600f, 1.1f, new MinMax(0.1f, 0.2f), new MinMax(0.1f, 0.2f), 1000f, 2.5f, new string[1] { "75" }, new string[9] { "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-1200-1300", "400-500-600-1000-1100-1200-1300", "400-500-600-700-800-900-1300", "400-500-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300", "400-500-600-700-800-900-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300" }, 1f, 1f, 0.5f), new Coaster(new MinMax(0.01f, 0.1f), 1f, 1050f, 2.25f, new MinMax(6f, 8f), new string[1] { "F,E,F,E,F,E,F,E,F" }, 0.3f), new Swing(300f, 200f, 0.5f, 0.7f, false, 500f, new MinMax(2.4f, 4.8f), 13f, 700f, new string[8] { "0,400,800,1200", "1150-775-375-50", "100-500-1100", "0,150,1050,1200", "50,500,700,1150", "1200,800,400,0", "1100-700-100", "0,200,1000,1200" }, 0.8f, 0.01f)));
				break;
			case Level.Mode.Hard:
				hp = 1850;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new BumperCar(525f, 1400f, new string[1] { "3.3,3.6,2.2,3.5,3,1.5,3.5,2,3.8,2,1.5" }, 0.6f, new string[1] { "F,F,B,B,F,B,F,B,B,F,B,F,F,B,F,B" }, 0.8f, 0.3f), new Duck(new MinMax(120f, 160f), new string[1] { "0,10,20,30,40" }, 210f, 420f, new string[2] { "R,B,R,R,B,R,P,B", "R,R,B,P,R,B,R,B" }, 2.6f, 12f, 350f), new HeliumClown(true, 7f, 300f, new string[2] { "2.5,2.5,2.8,2.5,2.5,2.5,2.8", "2.5,2.5,2.6,2.7,2.5,2.5,2.8" }, new string[3] { "R,R,R,R,P,R,R,R,R,R,P,R,R,R,P", "R,R,R,R,P,R,R,R,R,P,R,R,R,R,R,P", "R,R,R,R,R,R,P,R,R,R,R,P,R,R,R,R,R,P" }, new string[7] { "1-2-3,3-4,4-5-6,1-3,3-4,4-6", "2-3,4-5,3-4,1-2,5-6,2-3,4-5,3-4,1-2,5-6,1-3-4-6", "1-2,5-6,2-3,4-5,3-4,1-2,5-6,2-3,4-5,3-4,1-3-4-6", "3-6,4-1,2-5,3-6,4-1,2-5,1-3-4-6", "4-1,3-6,5-2,4-1,3-6,5-2,1-3-4-6", "1-6,2-5,3-4,1-6,2-5,3-4,1-3-4-6", "3-4,2-5,1-6,3-4,2-5,1-6,1-3-4-6" }, 200f, 15f, true), new Horse(true, 400f, new string[1] { "W,D,D,W,D,W,W,D" }, 200f, 3, 320f, 30f, 4f, 0.3f, 3f, 2f, new string[2] { "105,115,110", "100,110,105" }, new string[4] { "P,R,R", "R,R,R", "R,R,P", "R,R,R" }, 1f, 1600f, 0.9f, new MinMax(0.1f, 0.2f), new MinMax(0.1f, 0.2f), 1000f, 2.5f, new string[1] { "75" }, new string[10] { "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-900-1300", "400-500-600-700-800-1200-1300", "400-500-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-1200-1300", "400-500-600-700-800-900-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-1000-1100-1200-1300" }, 1f, 1f, 0.5f), new Coaster(new MinMax(2f, 3f), 1f, 450f, 1.25f, new MinMax(0.8f, 1.6f), new string[1] { "E,F,E,E,F,E,E,F,E" }, 1f), new Swing(360f, 200f, 0.5f, 0.7f, false, 500f, new MinMax(1.5f, 3.5f), 13f, 800f, new string[9] { "0,200,1000,1200", "100-500-1100", "1200,800,400,0", "0,150,1050,1200", "50,500,700,1150", "1100-700-100", "1150-775-375-50", "0,400,800,1200", "0,150,1050,1200" }, 0.8f, 0f)));
				list.Add(new State(0.83f, new Pattern[1][] { new Pattern[0] }, States.HeliumTank, new BumperCar(525f, 1400f, new string[1] { "3.3,3.6,2.2,3.5,3,1.5,3.5,2,3.8,2,1.5" }, 0.6f, new string[1] { "F,F,B,B,F,B,F,B,B,F,B,F,F,B,F,B" }, 0.8f, 0.3f), new Duck(new MinMax(120f, 160f), new string[1] { "0,10,20,30,40" }, 210f, 420f, new string[2] { "R,B,R,R,B,R,P,B", "R,R,B,P,R,B,R,B" }, 2.6f, 12f, 350f), new HeliumClown(true, 7f, 300f, new string[2] { "2.5,2.5,2.8,2.5,2.5,2.5,2.8", "2.5,2.5,2.6,2.7,2.5,2.5,2.8" }, new string[3] { "R,R,R,R,P,R,R,R,R,R,P,R,R,R,P", "R,R,R,R,P,R,R,R,R,P,R,R,R,R,R,P", "R,R,R,R,R,R,P,R,R,R,R,P,R,R,R,R,R,P" }, new string[7] { "1-2-3,3-4,4-5-6,1-3,3-4,4-6", "2-3,4-5,3-4,1-2,5-6,2-3,4-5,3-4,1-2,5-6,1-3-4-6", "1-2,5-6,2-3,4-5,3-4,1-2,5-6,2-3,4-5,3-4,1-3-4-6", "3-6,4-1,2-5,3-6,4-1,2-5,1-3-4-6", "4-1,3-6,5-2,4-1,3-6,5-2,1-3-4-6", "1-6,2-5,3-4,1-6,2-5,3-4,1-3-4-6", "3-4,2-5,1-6,3-4,2-5,1-6,1-3-4-6" }, 200f, 15f, true), new Horse(true, 400f, new string[1] { "W,D,D,W,D,W,W,D" }, 200f, 3, 320f, 30f, 4f, 0.3f, 3f, 2f, new string[2] { "105,115,110", "100,110,105" }, new string[4] { "P,R,R", "R,R,R", "R,R,P", "R,R,R" }, 1f, 1600f, 0.9f, new MinMax(0.1f, 0.2f), new MinMax(0.1f, 0.2f), 1000f, 2.5f, new string[1] { "75" }, new string[10] { "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-900-1300", "400-500-600-700-800-1200-1300", "400-500-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-1200-1300", "400-500-600-700-800-900-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-1000-1100-1200-1300" }, 1f, 1f, 0.5f), new Coaster(new MinMax(2f, 3f), 1f, 450f, 1.25f, new MinMax(0.8f, 1.6f), new string[1] { "E,F,E,E,F,E,E,F,E" }, 1f), new Swing(360f, 200f, 0.5f, 0.7f, false, 500f, new MinMax(1.5f, 3.5f), 13f, 800f, new string[9] { "0,200,1000,1200", "100-500-1100", "1200,800,400,0", "0,150,1050,1200", "50,500,700,1150", "1100-700-100", "1150-775-375-50", "0,400,800,1200", "0,150,1050,1200" }, 0.8f, 0f)));
				list.Add(new State(0.63f, new Pattern[1][] { new Pattern[0] }, States.CarouselHorse, new BumperCar(525f, 1400f, new string[1] { "3.3,3.6,2.2,3.5,3,1.5,3.5,2,3.8,2,1.5" }, 0.6f, new string[1] { "F,F,B,B,F,B,F,B,B,F,B,F,F,B,F,B" }, 0.8f, 0.3f), new Duck(new MinMax(120f, 160f), new string[1] { "0,10,20,30,40" }, 210f, 420f, new string[2] { "R,B,R,R,B,R,P,B", "R,R,B,P,R,B,R,B" }, 2.6f, 12f, 350f), new HeliumClown(true, 7f, 300f, new string[2] { "2.5,2.5,2.8,2.5,2.5,2.5,2.8", "2.5,2.5,2.6,2.7,2.5,2.5,2.8" }, new string[3] { "R,R,R,R,P,R,R,R,R,R,P,R,R,R,P", "R,R,R,R,P,R,R,R,R,P,R,R,R,R,R,P", "R,R,R,R,R,R,P,R,R,R,R,P,R,R,R,R,R,P" }, new string[7] { "1-2-3,3-4,4-5-6,1-3,3-4,4-6", "2-3,4-5,3-4,1-2,5-6,2-3,4-5,3-4,1-2,5-6,1-3-4-6", "1-2,5-6,2-3,4-5,3-4,1-2,5-6,2-3,4-5,3-4,1-3-4-6", "3-6,4-1,2-5,3-6,4-1,2-5,1-3-4-6", "4-1,3-6,5-2,4-1,3-6,5-2,1-3-4-6", "1-6,2-5,3-4,1-6,2-5,3-4,1-3-4-6", "3-4,2-5,1-6,3-4,2-5,1-6,1-3-4-6" }, 200f, 15f, true), new Horse(true, 400f, new string[1] { "W,D,D,W,D,W,W,D" }, 200f, 3, 320f, 30f, 4f, 0.3f, 3f, 2f, new string[2] { "105,115,110", "100,110,105" }, new string[4] { "P,R,R", "R,R,R", "R,R,P", "R,R,R" }, 1f, 1600f, 0.9f, new MinMax(0.1f, 0.2f), new MinMax(0.1f, 0.2f), 1000f, 2.5f, new string[1] { "75" }, new string[10] { "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-900-1300", "400-500-600-700-800-1200-1300", "400-500-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-1200-1300", "400-500-600-700-800-900-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-1000-1100-1200-1300" }, 1f, 1f, 0.5f), new Coaster(new MinMax(1.5f, 2.5f), 1f, 500f, 1.75f, new MinMax(1f, 1.6f), new string[1] { "E,F,E,E,F,E,E,F,E" }, 0.8f), new Swing(360f, 200f, 0.5f, 0.7f, false, 500f, new MinMax(1.5f, 3.5f), 13f, 800f, new string[9] { "0,200,1000,1200", "100-500-1100", "1200,800,400,0", "0,150,1050,1200", "50,500,700,1150", "1100-700-100", "1150-775-375-50", "0,400,800,1200", "0,150,1050,1200" }, 0.8f, 0f)));
				list.Add(new State(0.45f, new Pattern[1][] { new Pattern[0] }, States.Swing, new BumperCar(525f, 1400f, new string[1] { "3.3,3.6,2.2,3.5,3,1.5,3.5,2,3.8,2,1.5" }, 0.6f, new string[1] { "F,F,B,B,F,B,F,B,B,F,B,F,F,B,F,B" }, 0.8f, 0.3f), new Duck(new MinMax(120f, 160f), new string[1] { "0,10,20,30,40" }, 210f, 420f, new string[2] { "R,B,R,R,B,R,P,B", "R,R,B,P,R,B,R,B" }, 2.6f, 12f, 350f), new HeliumClown(true, 7f, 300f, new string[2] { "2.5,2.5,2.8,2.5,2.5,2.5,2.8", "2.5,2.5,2.6,2.7,2.5,2.5,2.8" }, new string[3] { "R,R,R,R,P,R,R,R,R,R,P,R,R,R,P", "R,R,R,R,P,R,R,R,R,P,R,R,R,R,R,P", "R,R,R,R,R,R,P,R,R,R,R,P,R,R,R,R,R,P" }, new string[7] { "1-2-3,3-4,4-5-6,1-3,3-4,4-6", "2-3,4-5,3-4,1-2,5-6,2-3,4-5,3-4,1-2,5-6,1-3-4-6", "1-2,5-6,2-3,4-5,3-4,1-2,5-6,2-3,4-5,3-4,1-3-4-6", "3-6,4-1,2-5,3-6,4-1,2-5,1-3-4-6", "4-1,3-6,5-2,4-1,3-6,5-2,1-3-4-6", "1-6,2-5,3-4,1-6,2-5,3-4,1-3-4-6", "3-4,2-5,1-6,3-4,2-5,1-6,1-3-4-6" }, 200f, 15f, true), new Horse(true, 400f, new string[1] { "W,D,D,W,D,W,W,D" }, 200f, 3, 320f, 30f, 4f, 0.3f, 3f, 2f, new string[2] { "105,115,110", "100,110,105" }, new string[4] { "P,R,R", "R,R,R", "R,R,P", "R,R,R" }, 1f, 1600f, 0.9f, new MinMax(0.1f, 0.2f), new MinMax(0.1f, 0.2f), 1000f, 2.5f, new string[1] { "75" }, new string[10] { "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-900-1300", "400-500-600-700-800-1200-1300", "400-500-900-1000-1100-1200-1300", "400-500-600-700-1100-1200-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-700-800-1200-1300", "400-500-600-700-800-900-1300", "400-800-900-1000-1100-1200-1300", "400-500-600-1000-1100-1200-1300" }, 1f, 1f, 0.5f), new Coaster(new MinMax(0.01f, 0.01f), 1f, 1200f, 2.25f, new MinMax(6f, 8f), new string[1] { "F,E,F,E,F,E,F,E,F" }, 0.3f), new Swing(360f, 200f, 0.5f, 0.7f, false, 500f, new MinMax(1.5f, 3.5f), 13f, 800f, new string[9] { "0,200,1000,1200", "100-500-1100", "1200,800,400,0", "0,150,1050,1200", "50,500,700,1150", "1100-700-100", "1150-775-375-50", "0,400,800,1200", "0,150,1050,1200" }, 0.8f, 0f)));
				break;
			}
			return new Clown(hp, goalTimes, list.ToArray());
		}
	}

	public class Devil : AbstractLevelProperties<Devil.State, Devil.Pattern, Devil.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Devil properties { get; private set; }

			public virtual void LevelInit(Devil properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			Split = 2,
			GiantHead = 3,
			Hands = 4,
			Tears = 5
		}

		public enum Pattern
		{
			Default = 0,
			SplitDevilProjectileAttack = 1,
			SplitDevilWallAttack = 2,
			Clap = 3,
			Head = 4,
			Pitchfork = 5,
			BombEye = 6,
			SkullEye = 7,
			Uninitialized = 8
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly SplitDevilWall splitDevilWall;

			public readonly SplitDevilProjectiles splitDevilProjectiles;

			public readonly Demons demons;

			public readonly Clap clap;

			public readonly Spider spider;

			public readonly Dragon dragon;

			public readonly Pitchfork pitchfork;

			public readonly PitchforkTwoFlameWheel pitchforkTwoFlameWheel;

			public readonly PitchforkThreeFlameJumper pitchforkThreeFlameJumper;

			public readonly PitchforkFourFlameBouncer pitchforkFourFlameBouncer;

			public readonly PitchforkFiveFlameSpinner pitchforkFiveFlameSpinner;

			public readonly PitchforkSixFlameRing pitchforkSixFlameRing;

			public readonly GiantHeadPlatforms giantHeadPlatforms;

			public readonly Fireballs fireballs;

			public readonly BombEye bombEye;

			public readonly SkullEye skullEye;

			public readonly Hands hands;

			public readonly Swoopers swoopers;

			public readonly Tears tears;

			public readonly Firewall firewall;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, SplitDevilWall splitDevilWall, SplitDevilProjectiles splitDevilProjectiles, Demons demons, Clap clap, Spider spider, Dragon dragon, Pitchfork pitchfork, PitchforkTwoFlameWheel pitchforkTwoFlameWheel, PitchforkThreeFlameJumper pitchforkThreeFlameJumper, PitchforkFourFlameBouncer pitchforkFourFlameBouncer, PitchforkFiveFlameSpinner pitchforkFiveFlameSpinner, PitchforkSixFlameRing pitchforkSixFlameRing, GiantHeadPlatforms giantHeadPlatforms, Fireballs fireballs, BombEye bombEye, SkullEye skullEye, Hands hands, Swoopers swoopers, Tears tears, Firewall firewall)
				: base(healthTrigger, patterns, stateName)
			{
				this.splitDevilWall = splitDevilWall;
				this.splitDevilProjectiles = splitDevilProjectiles;
				this.demons = demons;
				this.clap = clap;
				this.spider = spider;
				this.dragon = dragon;
				this.pitchfork = pitchfork;
				this.pitchforkTwoFlameWheel = pitchforkTwoFlameWheel;
				this.pitchforkThreeFlameJumper = pitchforkThreeFlameJumper;
				this.pitchforkFourFlameBouncer = pitchforkFourFlameBouncer;
				this.pitchforkFiveFlameSpinner = pitchforkFiveFlameSpinner;
				this.pitchforkSixFlameRing = pitchforkSixFlameRing;
				this.giantHeadPlatforms = giantHeadPlatforms;
				this.fireballs = fireballs;
				this.bombEye = bombEye;
				this.skullEye = skullEye;
				this.hands = hands;
				this.swoopers = swoopers;
				this.tears = tears;
				this.firewall = firewall;
			}
		}

		public class SplitDevilWall : AbstractLevelPropertyGroup
		{
			public readonly MinMax xRange;

			public readonly MinMax speed;

			public readonly MinMax hesitateAfterAttack;

			public SplitDevilWall(MinMax xRange, MinMax speed, MinMax hesitateAfterAttack)
			{
				this.xRange = xRange;
				this.speed = speed;
				this.hesitateAfterAttack = hesitateAfterAttack;
			}
		}

		public class SplitDevilProjectiles : AbstractLevelPropertyGroup
		{
			public readonly MinMax numProjectiles;

			public readonly MinMax delayBetweenProjectiles;

			public readonly float projectileSpeed;

			public readonly MinMax hesitateAfterAttack;

			public SplitDevilProjectiles(MinMax numProjectiles, MinMax delayBetweenProjectiles, float projectileSpeed, MinMax hesitateAfterAttack)
			{
				this.numProjectiles = numProjectiles;
				this.delayBetweenProjectiles = delayBetweenProjectiles;
				this.projectileSpeed = projectileSpeed;
				this.hesitateAfterAttack = hesitateAfterAttack;
			}
		}

		public class Demons : AbstractLevelPropertyGroup
		{
			public readonly float hp;

			public readonly float speed;

			public readonly float delay;

			public Demons(float hp, float speed, float delay)
			{
				this.hp = hp;
				this.speed = speed;
				this.delay = delay;
			}
		}

		public class Clap : AbstractLevelPropertyGroup
		{
			public readonly MinMax delay;

			public readonly float warning;

			public readonly float speed;

			public readonly float hesitate;

			public Clap(MinMax delay, float warning, float speed, float hesitate)
			{
				this.delay = delay;
				this.warning = warning;
				this.speed = speed;
				this.hesitate = hesitate;
			}
		}

		public class Spider : AbstractLevelPropertyGroup
		{
			public readonly float downSpeed;

			public readonly float upSpeed;

			public readonly string positionOffset;

			public readonly MinMax numAttacks;

			public readonly MinMax entranceDelay;

			public readonly float hesitate;

			public Spider(float downSpeed, float upSpeed, string positionOffset, MinMax numAttacks, MinMax entranceDelay, float hesitate)
			{
				this.downSpeed = downSpeed;
				this.upSpeed = upSpeed;
				this.positionOffset = positionOffset;
				this.numAttacks = numAttacks;
				this.entranceDelay = entranceDelay;
				this.hesitate = hesitate;
			}
		}

		public class Dragon : AbstractLevelPropertyGroup
		{
			public readonly float speed;

			public readonly float sinHeight;

			public readonly float sinSpeed;

			public readonly string positionOffset;

			public readonly float returnSpeed;

			public readonly float returnDelay;

			public readonly float hesitate;

			public Dragon(float speed, float sinHeight, float sinSpeed, string positionOffset, float returnSpeed, float returnDelay, float hesitate)
			{
				this.speed = speed;
				this.sinHeight = sinHeight;
				this.sinSpeed = sinSpeed;
				this.positionOffset = positionOffset;
				this.returnSpeed = returnSpeed;
				this.returnDelay = returnDelay;
				this.hesitate = hesitate;
			}
		}

		public class Pitchfork : AbstractLevelPropertyGroup
		{
			public readonly string[] patternString;

			public readonly float spawnCenterY;

			public readonly float spawnRadius;

			public readonly float dormantDuration;

			public Pitchfork(string[] patternString, float spawnCenterY, float spawnRadius, float dormantDuration)
			{
				this.patternString = patternString;
				this.spawnCenterY = spawnCenterY;
				this.spawnRadius = spawnRadius;
				this.dormantDuration = dormantDuration;
			}
		}

		public class PitchforkTwoFlameWheel : AbstractLevelPropertyGroup
		{
			public readonly string angleOffset;

			public readonly float rotationSpeed;

			public readonly float movementSpeed;

			public readonly MinMax initialtAttackDelay;

			public readonly MinMax secondAttackDelay;

			public readonly float hesitate;

			public PitchforkTwoFlameWheel(string angleOffset, float rotationSpeed, float movementSpeed, MinMax initialtAttackDelay, MinMax secondAttackDelay, float hesitate)
			{
				this.angleOffset = angleOffset;
				this.rotationSpeed = rotationSpeed;
				this.movementSpeed = movementSpeed;
				this.initialtAttackDelay = initialtAttackDelay;
				this.secondAttackDelay = secondAttackDelay;
				this.hesitate = hesitate;
			}
		}

		public class PitchforkThreeFlameJumper : AbstractLevelPropertyGroup
		{
			public readonly string angleOffset;

			public readonly MinMax launchAngle;

			public readonly MinMax launchSpeed;

			public readonly float gravity;

			public readonly MinMax initialAttackDelay;

			public readonly float jumpDelay;

			public readonly int numJumps;

			public readonly float hesitate;

			public PitchforkThreeFlameJumper(string angleOffset, MinMax launchAngle, MinMax launchSpeed, float gravity, MinMax initialAttackDelay, float jumpDelay, int numJumps, float hesitate)
			{
				this.angleOffset = angleOffset;
				this.launchAngle = launchAngle;
				this.launchSpeed = launchSpeed;
				this.gravity = gravity;
				this.initialAttackDelay = initialAttackDelay;
				this.jumpDelay = jumpDelay;
				this.numJumps = numJumps;
				this.hesitate = hesitate;
			}
		}

		public class PitchforkFourFlameBouncer : AbstractLevelPropertyGroup
		{
			public readonly string angleOffset;

			public readonly MinMax initialAttackDelay;

			public readonly float speed;

			public readonly int numBounces;

			public readonly float hesitate;

			public PitchforkFourFlameBouncer(string angleOffset, MinMax initialAttackDelay, float speed, int numBounces, float hesitate)
			{
				this.angleOffset = angleOffset;
				this.initialAttackDelay = initialAttackDelay;
				this.speed = speed;
				this.numBounces = numBounces;
				this.hesitate = hesitate;
			}
		}

		public class PitchforkFiveFlameSpinner : AbstractLevelPropertyGroup
		{
			public readonly string angleOffset;

			public readonly float rotationSpeed;

			public readonly float maxSpeed;

			public readonly float acceleration;

			public readonly float attackDuration;

			public readonly float hesitate;

			public PitchforkFiveFlameSpinner(string angleOffset, float rotationSpeed, float maxSpeed, float acceleration, float attackDuration, float hesitate)
			{
				this.angleOffset = angleOffset;
				this.rotationSpeed = rotationSpeed;
				this.maxSpeed = maxSpeed;
				this.acceleration = acceleration;
				this.attackDuration = attackDuration;
				this.hesitate = hesitate;
			}
		}

		public class PitchforkSixFlameRing : AbstractLevelPropertyGroup
		{
			public readonly string angleOffset;

			public readonly MinMax initialAttackDelay;

			public readonly float attackDelay;

			public readonly float speed;

			public readonly float groundDuration;

			public readonly float hesitate;

			public PitchforkSixFlameRing(string angleOffset, MinMax initialAttackDelay, float attackDelay, float speed, float groundDuration, float hesitate)
			{
				this.angleOffset = angleOffset;
				this.initialAttackDelay = initialAttackDelay;
				this.attackDelay = attackDelay;
				this.speed = speed;
				this.groundDuration = groundDuration;
				this.hesitate = hesitate;
			}
		}

		public class GiantHeadPlatforms : AbstractLevelPropertyGroup
		{
			public readonly float exitSpeed;

			public readonly float riseSpeed;

			public readonly string riseString;

			public readonly float maxHeight;

			public readonly float holdDelay;

			public readonly MinMax riseDelayRange;

			public readonly float size;

			public readonly bool riseDuringTearPhase;

			public GiantHeadPlatforms(float exitSpeed, float riseSpeed, string riseString, float maxHeight, float holdDelay, MinMax riseDelayRange, float size, bool riseDuringTearPhase)
			{
				this.exitSpeed = exitSpeed;
				this.riseSpeed = riseSpeed;
				this.riseString = riseString;
				this.maxHeight = maxHeight;
				this.holdDelay = holdDelay;
				this.riseDelayRange = riseDelayRange;
				this.size = size;
				this.riseDuringTearPhase = riseDuringTearPhase;
			}
		}

		public class Fireballs : AbstractLevelPropertyGroup
		{
			public readonly float initialDelay;

			public readonly float fallSpeed;

			public readonly float fallAcceleration;

			public readonly float spawnDelay;

			public readonly float size;

			public Fireballs(float initialDelay, float fallSpeed, float fallAcceleration, float spawnDelay, float size)
			{
				this.initialDelay = initialDelay;
				this.fallSpeed = fallSpeed;
				this.fallAcceleration = fallAcceleration;
				this.spawnDelay = spawnDelay;
				this.size = size;
			}
		}

		public class BombEye : AbstractLevelPropertyGroup
		{
			public readonly float xSinHeight;

			public readonly float ySinHeight;

			public readonly float xSinSpeed;

			public readonly float ySinSpeed;

			public readonly float explodeDelay;

			public readonly MinMax hesitate;

			public BombEye(float xSinHeight, float ySinHeight, float xSinSpeed, float ySinSpeed, float explodeDelay, MinMax hesitate)
			{
				this.xSinHeight = xSinHeight;
				this.ySinHeight = ySinHeight;
				this.xSinSpeed = xSinSpeed;
				this.ySinSpeed = ySinSpeed;
				this.explodeDelay = explodeDelay;
				this.hesitate = hesitate;
			}
		}

		public class SkullEye : AbstractLevelPropertyGroup
		{
			public readonly float initialMoveDuration;

			public readonly float initialMoveSpeed;

			public readonly float swirlMoveOutwardSpeed;

			public readonly float swirlRotationSpeed;

			public readonly MinMax hesitate;

			public SkullEye(float initialMoveDuration, float initialMoveSpeed, float swirlMoveOutwardSpeed, float swirlRotationSpeed, MinMax hesitate)
			{
				this.initialMoveDuration = initialMoveDuration;
				this.initialMoveSpeed = initialMoveSpeed;
				this.swirlMoveOutwardSpeed = swirlMoveOutwardSpeed;
				this.swirlRotationSpeed = swirlRotationSpeed;
				this.hesitate = hesitate;
			}
		}

		public class Hands : AbstractLevelPropertyGroup
		{
			public readonly float HP;

			public readonly MinMax yRange;

			public readonly float speed;

			public readonly MinMax shotDelay;

			public readonly float bulletSpeed;

			public readonly MinMax initialSpawnDelay;

			public readonly MinMax spawnDelayRange;

			public readonly string pinkString;

			public Hands(float HP, MinMax yRange, float speed, MinMax shotDelay, float bulletSpeed, MinMax initialSpawnDelay, MinMax spawnDelayRange, string pinkString)
			{
				this.HP = HP;
				this.yRange = yRange;
				this.speed = speed;
				this.shotDelay = shotDelay;
				this.bulletSpeed = bulletSpeed;
				this.initialSpawnDelay = initialSpawnDelay;
				this.spawnDelayRange = spawnDelayRange;
				this.pinkString = pinkString;
			}
		}

		public class Swoopers : AbstractLevelPropertyGroup
		{
			public readonly string positions;

			public readonly MinMax spawnCount;

			public readonly int maxCount;

			public readonly float hp;

			public readonly MinMax attackDelay;

			public readonly MinMax launchAngle;

			public readonly MinMax launchSpeed;

			public readonly float gravity;

			public readonly MinMax initialSpawnDelay;

			public readonly MinMax spawnDelay;

			public readonly MinMax yIdlePos;

			public Swoopers(string positions, MinMax spawnCount, int maxCount, float hp, MinMax attackDelay, MinMax launchAngle, MinMax launchSpeed, float gravity, MinMax initialSpawnDelay, MinMax spawnDelay, MinMax yIdlePos)
			{
				this.positions = positions;
				this.spawnCount = spawnCount;
				this.maxCount = maxCount;
				this.hp = hp;
				this.attackDelay = attackDelay;
				this.launchAngle = launchAngle;
				this.launchSpeed = launchSpeed;
				this.gravity = gravity;
				this.initialSpawnDelay = initialSpawnDelay;
				this.spawnDelay = spawnDelay;
				this.yIdlePos = yIdlePos;
			}
		}

		public class Tears : AbstractLevelPropertyGroup
		{
			public readonly float speed;

			public readonly float delay;

			public Tears(float speed, float delay)
			{
				this.speed = speed;
				this.delay = delay;
			}
		}

		public class Firewall : AbstractLevelPropertyGroup
		{
			public readonly float firewallSpeed;

			public Firewall(float firewallSpeed)
			{
				this.firewallSpeed = firewallSpeed;
			}
		}

		[CompilerGenerated]
		private static Dictionary<string, int> _003C_003Ef__switch_0024map7;

		public Devil(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1250f;
				break;
			case Level.Mode.Normal:
				timeline.health = 1900f;
				timeline.events.Add(new Level.Timeline.Event("GiantHead", 0.65f));
				timeline.events.Add(new Level.Timeline.Event("Hands", 0.35f));
				timeline.events.Add(new Level.Timeline.Event("Tears", 0.1f));
				break;
			case Level.Mode.Hard:
				timeline.health = 2100f;
				timeline.events.Add(new Level.Timeline.Event("GiantHead", 0.65f));
				timeline.events.Add(new Level.Timeline.Event("Hands", 0.35f));
				timeline.events.Add(new Level.Timeline.Event("Tears", 0.1f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null)
			{
				if (_003C_003Ef__switch_0024map7 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(8);
					dictionary.Add("D", 0);
					dictionary.Add("P", 1);
					dictionary.Add("W", 2);
					dictionary.Add("C", 3);
					dictionary.Add("H", 4);
					dictionary.Add("F", 5);
					dictionary.Add("B", 6);
					dictionary.Add("S", 7);
					_003C_003Ef__switch_0024map7 = dictionary;
				}
				int value;
				if (_003C_003Ef__switch_0024map7.TryGetValue(id, out value))
				{
					switch (value)
					{
					case 0:
						return Pattern.Default;
					case 1:
						return Pattern.SplitDevilProjectileAttack;
					case 2:
						return Pattern.SplitDevilWallAttack;
					case 3:
						return Pattern.Clap;
					case 4:
						return Pattern.Head;
					case 5:
						return Pattern.Pitchfork;
					case 6:
						return Pattern.BombEye;
					case 7:
						return Pattern.SkullEye;
					}
				}
			}
			Debug.LogError("Pattern Devil.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static Devil GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1250;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[7]
				{
					Pattern.Head,
					Pattern.Clap,
					Pattern.Pitchfork,
					Pattern.Clap,
					Pattern.Head,
					Pattern.Clap,
					Pattern.Pitchfork
				} }, States.Main, new SplitDevilWall(new MinMax(-350f, 350f), new MinMax(275f, 315f), new MinMax(2f, 2.5f)), new SplitDevilProjectiles(new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new MinMax(0f, 1f)), new Demons(3.5f, 550f, 4.5f), new Clap(new MinMax(0.1f, 0.5f), 0.5f, 0.2f, 1.5f), new Spider(805f, 1050f, "-150, 50, -50, 300, -200, 50, 150, -300, 0, 100, -50, 200, 50, 0, 100, -150, 50, -250, 200, 0", new MinMax(3f, 6f), new MinMax(0.3f, 0.7f), 1.5f), new Dragon(500f, 300f, 6f, "0, 150, 50, 200, 0, 100, 200, 50", 1600f, 0.5f, 1.5f), new Pitchfork(new string[3] { "4, 6, 5", "5, 4, 6, 4", "4, 5, 6" }, 50f, 300f, 1f), new PitchforkTwoFlameWheel(string.Empty, 0f, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f), new PitchforkThreeFlameJumper(string.Empty, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), 0f, 0, 0f), new PitchforkFourFlameBouncer("55, 30, 35, 60, 40, 70, 20, 35, 50, 100, 200", new MinMax(1f, 1.5f), 850f, 6, 1.5f), new PitchforkFiveFlameSpinner("0, 10, -10, 0, 5, -5, -20, 20, 0, 30, -30", 85f, 275f, 350f, 5f, 1.5f), new PitchforkSixFlameRing("0, 10, -10, 25, 5, 30, -25, 15, 35, 20, -20", new MinMax(1f, 1.5f), 0.7f, 600f, 0f, 1.5f), new GiantHeadPlatforms(0f, 0f, string.Empty, 0f, 0f, new MinMax(0f, 1f), 0f, false), new Fireballs(0f, 0f, 0f, 0f, 0f), new BombEye(0f, 0f, 0f, 0f, 0f, new MinMax(0f, 1f)), new SkullEye(0f, 0f, 0f, 0f, new MinMax(0f, 1f)), new Hands(0f, new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), string.Empty), new Swoopers(string.Empty, new MinMax(0f, 1f), 0, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), new MinMax(0f, 1f)), new Tears(0f, 0f), new Firewall(0f)));
				break;
			case Level.Mode.Normal:
				hp = 1900;
				goalTimes = new Level.GoalTimes(180f, 180f, 180f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[7]
				{
					Pattern.Head,
					Pattern.Clap,
					Pattern.Pitchfork,
					Pattern.Clap,
					Pattern.Head,
					Pattern.Clap,
					Pattern.Pitchfork
				} }, States.Main, new SplitDevilWall(new MinMax(-350f, 350f), new MinMax(275f, 315f), new MinMax(2f, 2.5f)), new SplitDevilProjectiles(new MinMax(3f, 6f), new MinMax(0.55f, 0.65f), 586f, new MinMax(0.5f, 1.5f)), new Demons(4.5f, 550f, 4.5f), new Clap(new MinMax(0.1f, 0.5f), 0.5f, 0.2f, 1.5f), new Spider(805f, 1050f, "-150, 50, -50, 300, -200, 50, 150, -300, 0, 100, -50, 200, 50, 0, 100, -150, 50, -250, 200, 0", new MinMax(3f, 6f), new MinMax(0.3f, 0.7f), 1.5f), new Dragon(1f, 300f, 6f, "0, 150, 50, 200, 0, 100, 200, 50", 1600f, 0.5f, 1.5f), new Pitchfork(new string[3] { "4, 6, 5", "5, 4, 6, 4", "4, 5, 6" }, 50f, 300f, 1f), new PitchforkTwoFlameWheel("90, 100, 80", 360f, 600f, new MinMax(1f, 1.5f), new MinMax(1.5f, 2.5f), 1.5f), new PitchforkThreeFlameJumper("0, 10, -10, 50, -50, 100, -100, 75, -75", new MinMax(45f, 90f), new MinMax(1200f, 1250f), 1500f, new MinMax(1f, 1.5f), 0.8f, 3, 1.5f), new PitchforkFourFlameBouncer("55, 30, 35, 60, 40, 70, 20, 35, 50, 100, 200", new MinMax(1f, 1.5f), 725f, 6, 1.5f), new PitchforkFiveFlameSpinner("0, 10, -10, 0, 5, -5, -20, 20, 0, 30, -30", 85f, 275f, 350f, 5f, 1.5f), new PitchforkSixFlameRing("0, 10, -10, 25, 5, 30, -25, 15, 35, 20, -20", new MinMax(1f, 1.5f), 0.7f, 565f, 0f, 1.5f), new GiantHeadPlatforms(185f, 115f, "1,3,2,5,4,1,5,4,2,3,1,3,5,4,2", 165f, 1f, new MinMax(2f, 3.5f), 200f, false), new Fireballs(2f, 215f, 515f, 3.5f, 220f), new BombEye(200f, 100f, 2f, 5f, 2.2f, new MinMax(6.5f, 8f)), new SkullEye(0.75f, 315f, 165f, 198f, new MinMax(5f, 6.5f)), new Hands(42f, new MinMax(-180f, 100f), 175f, new MinMax(2.75f, 4.25f), 415f, new MinMax(1f, 2f), new MinMax(6.5f, 7.8f), "R,R,P"), new Swoopers("0,300,600,200,800,1000,500,1100,100,900,400,700,1200", new MinMax(3f, 4f), 5, 3.5f, new MinMax(1.8f, 2.9f), new MinMax(45f, 90f), new MinMax(1200f, 800f), 1000f, new MinMax(2f, 3f), new MinMax(6f, 8f), new MinMax(220f, 300f)), new Tears(465f, 1f), new Firewall(300f)));
				list.Add(new State(0.65f, new Pattern[1][] { new Pattern[18]
				{
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.SkullEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.SkullEye
				} }, States.GiantHead, new SplitDevilWall(new MinMax(-350f, 350f), new MinMax(275f, 315f), new MinMax(2f, 2.5f)), new SplitDevilProjectiles(new MinMax(3f, 6f), new MinMax(0.55f, 0.65f), 586f, new MinMax(0.5f, 1.5f)), new Demons(4.5f, 550f, 4.5f), new Clap(new MinMax(0.1f, 0.5f), 0.5f, 0.2f, 1.5f), new Spider(805f, 1050f, "-150, 50, -50, 300, -200, 50, 150, -300, 0, 100, -50, 200, 50, 0, 100, -150, 50, -250, 200, 0", new MinMax(3f, 6f), new MinMax(0.3f, 0.7f), 1.5f), new Dragon(1f, 300f, 6f, "0, 150, 50, 200, 0, 100, 200, 50", 1600f, 0.5f, 1.5f), new Pitchfork(new string[3] { "4, 6, 5", "5, 4, 6, 4", "4, 5, 6" }, 50f, 300f, 1f), new PitchforkTwoFlameWheel("90, 100, 80", 360f, 600f, new MinMax(1f, 1.5f), new MinMax(1.5f, 2.5f), 1.5f), new PitchforkThreeFlameJumper("0, 10, -10, 50, -50, 100, -100, 75, -75", new MinMax(45f, 90f), new MinMax(1200f, 1250f), 1500f, new MinMax(1f, 1.5f), 0.8f, 3, 1.5f), new PitchforkFourFlameBouncer("55, 30, 35, 60, 40, 70, 20, 35, 50, 100, 200", new MinMax(1f, 1.5f), 725f, 6, 1.5f), new PitchforkFiveFlameSpinner("0, 10, -10, 0, 5, -5, -20, 20, 0, 30, -30", 85f, 275f, 350f, 5f, 1.5f), new PitchforkSixFlameRing("0, 10, -10, 25, 5, 30, -25, 15, 35, 20, -20", new MinMax(1f, 1.5f), 0.7f, 565f, 0f, 1.5f), new GiantHeadPlatforms(185f, 115f, "1,3,2,5,4,1,5,4,2,3,1,3,5,4,2", 165f, 1f, new MinMax(2f, 3.5f), 200f, false), new Fireballs(2f, 215f, 515f, 3.5f, 220f), new BombEye(200f, 100f, 2f, 5f, 2.2f, new MinMax(6.5f, 8f)), new SkullEye(0.75f, 315f, 165f, 198f, new MinMax(5f, 6.5f)), new Hands(42f, new MinMax(-180f, 100f), 175f, new MinMax(2.75f, 4.25f), 415f, new MinMax(1f, 2f), new MinMax(6.5f, 7.8f), "R,R,P"), new Swoopers("0,300,600,200,800,1000,500,1100,100,900,400,700,1200", new MinMax(3f, 4f), 5, 3.5f, new MinMax(1.8f, 2.9f), new MinMax(45f, 90f), new MinMax(1200f, 800f), 1000f, new MinMax(2f, 3f), new MinMax(6f, 8f), new MinMax(220f, 300f)), new Tears(465f, 1f), new Firewall(300f)));
				list.Add(new State(0.35f, new Pattern[1][] { new Pattern[0] }, States.Hands, new SplitDevilWall(new MinMax(-350f, 350f), new MinMax(275f, 315f), new MinMax(2f, 2.5f)), new SplitDevilProjectiles(new MinMax(3f, 6f), new MinMax(0.55f, 0.65f), 586f, new MinMax(0.5f, 1.5f)), new Demons(4.5f, 550f, 4.5f), new Clap(new MinMax(0.1f, 0.5f), 0.5f, 0.2f, 1.5f), new Spider(805f, 1050f, "-150, 50, -50, 300, -200, 50, 150, -300, 0, 100, -50, 200, 50, 0, 100, -150, 50, -250, 200, 0", new MinMax(3f, 6f), new MinMax(0.3f, 0.7f), 1.5f), new Dragon(1f, 300f, 6f, "0, 150, 50, 200, 0, 100, 200, 50", 1600f, 0.5f, 1.5f), new Pitchfork(new string[3] { "4, 6, 5", "5, 4, 6, 4", "4, 5, 6" }, 50f, 300f, 1f), new PitchforkTwoFlameWheel("90, 100, 80", 360f, 600f, new MinMax(1f, 1.5f), new MinMax(1.5f, 2.5f), 1.5f), new PitchforkThreeFlameJumper("0, 10, -10, 50, -50, 100, -100, 75, -75", new MinMax(45f, 90f), new MinMax(1200f, 1250f), 1500f, new MinMax(1f, 1.5f), 0.8f, 3, 1.5f), new PitchforkFourFlameBouncer("55, 30, 35, 60, 40, 70, 20, 35, 50, 100, 200", new MinMax(1f, 1.5f), 725f, 6, 1.5f), new PitchforkFiveFlameSpinner("0, 10, -10, 0, 5, -5, -20, 20, 0, 30, -30", 85f, 275f, 350f, 5f, 1.5f), new PitchforkSixFlameRing("0, 10, -10, 25, 5, 30, -25, 15, 35, 20, -20", new MinMax(1f, 1.5f), 0.7f, 565f, 0f, 1.5f), new GiantHeadPlatforms(185f, 115f, "1,3,2,5,4,1,5,4,2,3,1,3,5,4,2", 165f, 1f, new MinMax(2f, 3.5f), 200f, false), new Fireballs(2f, 215f, 515f, 3.5f, 220f), new BombEye(200f, 100f, 2f, 5f, 2.2f, new MinMax(6.5f, 8f)), new SkullEye(0.75f, 315f, 165f, 198f, new MinMax(5f, 6.5f)), new Hands(42f, new MinMax(-180f, 100f), 175f, new MinMax(2.75f, 4.25f), 415f, new MinMax(1f, 2f), new MinMax(6.5f, 7.8f), "R,R,P"), new Swoopers("0,300,600,200,800,1000,500,1100,100,900,400,700,1200", new MinMax(3f, 4f), 5, 3.5f, new MinMax(1.8f, 2.9f), new MinMax(45f, 90f), new MinMax(1200f, 800f), 1000f, new MinMax(2f, 3f), new MinMax(6f, 8f), new MinMax(220f, 300f)), new Tears(465f, 1f), new Firewall(300f)));
				list.Add(new State(0.1f, new Pattern[1][] { new Pattern[0] }, States.Tears, new SplitDevilWall(new MinMax(-350f, 350f), new MinMax(275f, 315f), new MinMax(2f, 2.5f)), new SplitDevilProjectiles(new MinMax(3f, 6f), new MinMax(0.55f, 0.65f), 586f, new MinMax(0.5f, 1.5f)), new Demons(4.5f, 550f, 4.5f), new Clap(new MinMax(0.1f, 0.5f), 0.5f, 0.2f, 1.5f), new Spider(805f, 1050f, "-150, 50, -50, 300, -200, 50, 150, -300, 0, 100, -50, 200, 50, 0, 100, -150, 50, -250, 200, 0", new MinMax(3f, 6f), new MinMax(0.3f, 0.7f), 1.5f), new Dragon(1f, 300f, 6f, "0, 150, 50, 200, 0, 100, 200, 50", 1600f, 0.5f, 1.5f), new Pitchfork(new string[3] { "4, 6, 5", "5, 4, 6, 4", "4, 5, 6" }, 50f, 300f, 1f), new PitchforkTwoFlameWheel("90, 100, 80", 360f, 600f, new MinMax(1f, 1.5f), new MinMax(1.5f, 2.5f), 1.5f), new PitchforkThreeFlameJumper("0, 10, -10, 50, -50, 100, -100, 75, -75", new MinMax(45f, 90f), new MinMax(1200f, 1250f), 1500f, new MinMax(1f, 1.5f), 0.8f, 3, 1.5f), new PitchforkFourFlameBouncer("55, 30, 35, 60, 40, 70, 20, 35, 50, 100, 200", new MinMax(1f, 1.5f), 725f, 6, 1.5f), new PitchforkFiveFlameSpinner("0, 10, -10, 0, 5, -5, -20, 20, 0, 30, -30", 85f, 275f, 350f, 5f, 1.5f), new PitchforkSixFlameRing("0, 10, -10, 25, 5, 30, -25, 15, 35, 20, -20", new MinMax(1f, 1.5f), 0.7f, 565f, 0f, 1.5f), new GiantHeadPlatforms(185f, 115f, "1,3,2,5,4,1,5,4,2,3,1,3,5,4,2", 165f, 1f, new MinMax(2f, 3.5f), 200f, false), new Fireballs(2f, 215f, 515f, 3.5f, 220f), new BombEye(200f, 100f, 2f, 5f, 2.2f, new MinMax(6.5f, 8f)), new SkullEye(0.75f, 315f, 165f, 198f, new MinMax(5f, 6.5f)), new Hands(42f, new MinMax(-180f, 100f), 175f, new MinMax(2.75f, 4.25f), 415f, new MinMax(1f, 2f), new MinMax(6.5f, 7.8f), "R,R,P"), new Swoopers("0,300,600,200,800,1000,500,1100,100,900,400,700,1200", new MinMax(3f, 4f), 5, 3.5f, new MinMax(1.8f, 2.9f), new MinMax(45f, 90f), new MinMax(1200f, 800f), 1000f, new MinMax(2f, 3f), new MinMax(6f, 8f), new MinMax(220f, 300f)), new Tears(465f, 1f), new Firewall(300f)));
				break;
			case Level.Mode.Hard:
				hp = 2100;
				goalTimes = new Level.GoalTimes(180f, 180f, 180f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[7]
				{
					Pattern.Head,
					Pattern.Clap,
					Pattern.Pitchfork,
					Pattern.Clap,
					Pattern.Head,
					Pattern.Clap,
					Pattern.Pitchfork
				} }, States.Main, new SplitDevilWall(new MinMax(-350f, 350f), new MinMax(315f, 345f), new MinMax(1.8f, 2.2f)), new SplitDevilProjectiles(new MinMax(3f, 7f), new MinMax(0.4f, 0.5f), 606f, new MinMax(0.5f, 1.5f)), new Demons(5f, 575f, 3.5f), new Clap(new MinMax(0.1f, 0.5f), 0.5f, 0.2f, 1.5f), new Spider(950f, 1150f, "-150, 50, -50, 300, -200, 50, 150, -300, 0, 100, -50, 200, 50, 0, 100, -150, 50, -250, 200, 0", new MinMax(3f, 6f), new MinMax(0.3f, 0.7f), 1.5f), new Dragon(1f, 300f, 6f, "0, 150, 50, 200, 0, 100, 200, 50", 1600f, 0.5f, 1.5f), new Pitchfork(new string[3] { "4, 5, 6", "6, 5, 4, 5", "4, 6, 5" }, 50f, 300f, 1f), new PitchforkTwoFlameWheel(string.Empty, 0f, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f), new PitchforkThreeFlameJumper(string.Empty, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), 0f, 0, 0f), new PitchforkFourFlameBouncer("55, 30, 35, 60, 40, 70, 20, 35, 50, 100, 200, 35, 60, 35, 55, 100, 70, 20, 30, 50, 40, 200", new MinMax(1f, 1.5f), 885f, 6, 1.5f), new PitchforkFiveFlameSpinner("0, 10, -10, 0, 5, -5, -20, 20, 0, 30, -30", 115f, 275f, 415f, 5.5f, 1.5f), new PitchforkSixFlameRing("0, 10, -10, 25, 5, 30, -25, 15, 35, 20, -20", new MinMax(1f, 1.5f), 0.55f, 635f, 0f, 1.5f), new GiantHeadPlatforms(185f, 125f, "1,3,2,5,4,1,5,4,2,3,1,3,5,4,2", 150f, 1f, new MinMax(1.5f, 2.8f), 200f, false), new Fireballs(2f, 240f, 550f, 2.8f, 220f), new BombEye(200f, 100f, 2f, 5f, 1.6f, new MinMax(6f, 7.5f)), new SkullEye(0.75f, 335f, 180f, 216f, new MinMax(4.8f, 6f)), new Hands(50f, new MinMax(-180f, 100f), 175f, new MinMax(2.5f, 3.8f), 435f, new MinMax(1f, 2f), new MinMax(6f, 7.3f), "R,R,P"), new Swoopers("0,300,600,200,800,1000,500,1100,100,900,400,700,1200", new MinMax(3f, 4f), 7, 3.5f, new MinMax(1.8f, 2.9f), new MinMax(45f, 90f), new MinMax(1200f, 800f), 1000f, new MinMax(2f, 3f), new MinMax(4.5f, 6f), new MinMax(220f, 300f)), new Tears(485f, 0.4f), new Firewall(300f)));
				list.Add(new State(0.65f, new Pattern[1][] { new Pattern[18]
				{
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.BombEye,
					Pattern.SkullEye,
					Pattern.SkullEye
				} }, States.GiantHead, new SplitDevilWall(new MinMax(-350f, 350f), new MinMax(315f, 345f), new MinMax(1.8f, 2.2f)), new SplitDevilProjectiles(new MinMax(3f, 7f), new MinMax(0.4f, 0.5f), 606f, new MinMax(0.5f, 1.5f)), new Demons(5f, 575f, 3.5f), new Clap(new MinMax(0.1f, 0.5f), 0.5f, 0.2f, 1.5f), new Spider(950f, 1150f, "-150, 50, -50, 300, -200, 50, 150, -300, 0, 100, -50, 200, 50, 0, 100, -150, 50, -250, 200, 0", new MinMax(3f, 6f), new MinMax(0.3f, 0.7f), 1.5f), new Dragon(1f, 300f, 6f, "0, 150, 50, 200, 0, 100, 200, 50", 1600f, 0.5f, 1.5f), new Pitchfork(new string[3] { "4, 5, 6", "6, 5, 4, 5", "4, 6, 5" }, 50f, 300f, 1f), new PitchforkTwoFlameWheel(string.Empty, 0f, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f), new PitchforkThreeFlameJumper(string.Empty, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), 0f, 0, 0f), new PitchforkFourFlameBouncer("55, 30, 35, 60, 40, 70, 20, 35, 50, 100, 200, 35, 60, 35, 55, 100, 70, 20, 30, 50, 40, 200", new MinMax(1f, 1.5f), 885f, 6, 1.5f), new PitchforkFiveFlameSpinner("0, 10, -10, 0, 5, -5, -20, 20, 0, 30, -30", 115f, 275f, 415f, 5.5f, 1.5f), new PitchforkSixFlameRing("0, 10, -10, 25, 5, 30, -25, 15, 35, 20, -20", new MinMax(1f, 1.5f), 0.55f, 635f, 0f, 1.5f), new GiantHeadPlatforms(185f, 125f, "1,3,2,5,4,1,5,4,2,3,1,3,5,4,2", 150f, 1f, new MinMax(1.5f, 2.8f), 200f, false), new Fireballs(2f, 240f, 550f, 2.8f, 220f), new BombEye(200f, 100f, 2f, 5f, 1.6f, new MinMax(6f, 7.5f)), new SkullEye(0.75f, 335f, 180f, 216f, new MinMax(4.8f, 6f)), new Hands(50f, new MinMax(-180f, 100f), 175f, new MinMax(2.5f, 3.8f), 435f, new MinMax(1f, 2f), new MinMax(6f, 7.3f), "R,R,P"), new Swoopers("0,300,600,200,800,1000,500,1100,100,900,400,700,1200", new MinMax(3f, 4f), 7, 3.5f, new MinMax(1.8f, 2.9f), new MinMax(45f, 90f), new MinMax(1200f, 800f), 1000f, new MinMax(2f, 3f), new MinMax(4.5f, 6f), new MinMax(220f, 300f)), new Tears(485f, 0.4f), new Firewall(300f)));
				list.Add(new State(0.35f, new Pattern[1][] { new Pattern[0] }, States.Hands, new SplitDevilWall(new MinMax(-350f, 350f), new MinMax(315f, 345f), new MinMax(1.8f, 2.2f)), new SplitDevilProjectiles(new MinMax(3f, 7f), new MinMax(0.4f, 0.5f), 606f, new MinMax(0.5f, 1.5f)), new Demons(5f, 575f, 3.5f), new Clap(new MinMax(0.1f, 0.5f), 0.5f, 0.2f, 1.5f), new Spider(950f, 1150f, "-150, 50, -50, 300, -200, 50, 150, -300, 0, 100, -50, 200, 50, 0, 100, -150, 50, -250, 200, 0", new MinMax(3f, 6f), new MinMax(0.3f, 0.7f), 1.5f), new Dragon(1f, 300f, 6f, "0, 150, 50, 200, 0, 100, 200, 50", 1600f, 0.5f, 1.5f), new Pitchfork(new string[3] { "4, 5, 6", "6, 5, 4, 5", "4, 6, 5" }, 50f, 300f, 1f), new PitchforkTwoFlameWheel(string.Empty, 0f, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f), new PitchforkThreeFlameJumper(string.Empty, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), 0f, 0, 0f), new PitchforkFourFlameBouncer("55, 30, 35, 60, 40, 70, 20, 35, 50, 100, 200, 35, 60, 35, 55, 100, 70, 20, 30, 50, 40, 200", new MinMax(1f, 1.5f), 885f, 6, 1.5f), new PitchforkFiveFlameSpinner("0, 10, -10, 0, 5, -5, -20, 20, 0, 30, -30", 115f, 275f, 415f, 5.5f, 1.5f), new PitchforkSixFlameRing("0, 10, -10, 25, 5, 30, -25, 15, 35, 20, -20", new MinMax(1f, 1.5f), 0.55f, 635f, 0f, 1.5f), new GiantHeadPlatforms(185f, 125f, "1,3,2,5,4,1,5,4,2,3,1,3,5,4,2", 150f, 1f, new MinMax(1.5f, 2.8f), 200f, false), new Fireballs(2f, 240f, 550f, 2.8f, 220f), new BombEye(200f, 100f, 2f, 5f, 1.6f, new MinMax(6f, 7.5f)), new SkullEye(0.75f, 335f, 180f, 216f, new MinMax(4.8f, 6f)), new Hands(50f, new MinMax(-180f, 100f), 175f, new MinMax(2.5f, 3.8f), 435f, new MinMax(1f, 2f), new MinMax(6f, 7.3f), "R,R,P"), new Swoopers("0,300,600,200,800,1000,500,1100,100,900,400,700,1200", new MinMax(3f, 4f), 7, 3.5f, new MinMax(1.8f, 2.9f), new MinMax(45f, 90f), new MinMax(1200f, 800f), 1000f, new MinMax(2f, 3f), new MinMax(4.5f, 6f), new MinMax(220f, 300f)), new Tears(485f, 0.4f), new Firewall(300f)));
				list.Add(new State(0.1f, new Pattern[1][] { new Pattern[0] }, States.Tears, new SplitDevilWall(new MinMax(-350f, 350f), new MinMax(315f, 345f), new MinMax(1.8f, 2.2f)), new SplitDevilProjectiles(new MinMax(3f, 7f), new MinMax(0.4f, 0.5f), 606f, new MinMax(0.5f, 1.5f)), new Demons(5f, 575f, 3.5f), new Clap(new MinMax(0.1f, 0.5f), 0.5f, 0.2f, 1.5f), new Spider(950f, 1150f, "-150, 50, -50, 300, -200, 50, 150, -300, 0, 100, -50, 200, 50, 0, 100, -150, 50, -250, 200, 0", new MinMax(3f, 6f), new MinMax(0.3f, 0.7f), 1.5f), new Dragon(1f, 300f, 6f, "0, 150, 50, 200, 0, 100, 200, 50", 1600f, 0.5f, 1.5f), new Pitchfork(new string[3] { "4, 5, 6", "6, 5, 4, 5", "4, 6, 5" }, 50f, 300f, 1f), new PitchforkTwoFlameWheel(string.Empty, 0f, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f), new PitchforkThreeFlameJumper(string.Empty, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), 0f, 0, 0f), new PitchforkFourFlameBouncer("55, 30, 35, 60, 40, 70, 20, 35, 50, 100, 200, 35, 60, 35, 55, 100, 70, 20, 30, 50, 40, 200", new MinMax(1f, 1.5f), 885f, 6, 1.5f), new PitchforkFiveFlameSpinner("0, 10, -10, 0, 5, -5, -20, 20, 0, 30, -30", 115f, 275f, 415f, 5.5f, 1.5f), new PitchforkSixFlameRing("0, 10, -10, 25, 5, 30, -25, 15, 35, 20, -20", new MinMax(1f, 1.5f), 0.55f, 635f, 0f, 1.5f), new GiantHeadPlatforms(185f, 125f, "1,3,2,5,4,1,5,4,2,3,1,3,5,4,2", 150f, 1f, new MinMax(1.5f, 2.8f), 200f, false), new Fireballs(2f, 240f, 550f, 2.8f, 220f), new BombEye(200f, 100f, 2f, 5f, 1.6f, new MinMax(6f, 7.5f)), new SkullEye(0.75f, 335f, 180f, 216f, new MinMax(4.8f, 6f)), new Hands(50f, new MinMax(-180f, 100f), 175f, new MinMax(2.5f, 3.8f), 435f, new MinMax(1f, 2f), new MinMax(6f, 7.3f), "R,R,P"), new Swoopers("0,300,600,200,800,1000,500,1100,100,900,400,700,1200", new MinMax(3f, 4f), 7, 3.5f, new MinMax(1.8f, 2.9f), new MinMax(45f, 90f), new MinMax(1200f, 800f), 1000f, new MinMax(2f, 3f), new MinMax(4.5f, 6f), new MinMax(220f, 300f)), new Tears(485f, 0.4f), new Firewall(300f)));
				break;
			}
			return new Devil(hp, goalTimes, list.ToArray());
		}
	}

	public class DiceGate : AbstractLevelProperties<DiceGate.State, DiceGate.Pattern, DiceGate.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DiceGate properties { get; private set; }

			public virtual void LevelInit(DiceGate properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public State(float healthTrigger, Pattern[][] patterns, States stateName)
				: base(healthTrigger, patterns, stateName)
			{
			}
		}

		public DiceGate(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 100f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DiceGate.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DiceGate GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			case Level.Mode.Normal:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			}
			return new DiceGate(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceBooze : AbstractLevelProperties<DicePalaceBooze.State, DicePalaceBooze.Pattern, DicePalaceBooze.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceBooze properties { get; private set; }

			public virtual void LevelInit(DicePalaceBooze properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Decanter decanter;

			public readonly Tumbler tumbler;

			public readonly Martini martini;

			public readonly Main main;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Decanter decanter, Tumbler tumbler, Martini martini, Main main)
				: base(healthTrigger, patterns, stateName)
			{
				this.decanter = decanter;
				this.tumbler = tumbler;
				this.martini = martini;
				this.main = main;
			}
		}

		public class Decanter : AbstractLevelPropertyGroup
		{
			public readonly float decanterHP;

			public readonly float beamDropSpeed;

			public readonly string attackDelayString;

			public readonly MinMax beamAppearDelayRange;

			public Decanter(float decanterHP, float beamDropSpeed, string attackDelayString, MinMax beamAppearDelayRange)
			{
				this.decanterHP = decanterHP;
				this.beamDropSpeed = beamDropSpeed;
				this.attackDelayString = attackDelayString;
				this.beamAppearDelayRange = beamAppearDelayRange;
			}
		}

		public class Tumbler : AbstractLevelPropertyGroup
		{
			public readonly float tumblerHP;

			public readonly string beamDelayString;

			public readonly float beamDuration;

			public readonly float beamWarningDuration;

			public Tumbler(float tumblerHP, string beamDelayString, float beamDuration, float beamWarningDuration)
			{
				this.tumblerHP = tumblerHP;
				this.beamDelayString = beamDelayString;
				this.beamDuration = beamDuration;
				this.beamWarningDuration = beamWarningDuration;
			}
		}

		public class Martini : AbstractLevelPropertyGroup
		{
			public readonly float martiniHP;

			public readonly int oliveHP;

			public readonly float oliveSpawnDelay;

			public readonly string moveString;

			public readonly float oliveSpeed;

			public readonly float oliveStopDuration;

			public readonly string[] olivePositionStringY;

			public readonly string[] olivePositionStringX;

			public readonly float bulletSpeed;

			public readonly string pinkString;

			public readonly float oliveHesitateAfterShooting;

			public Martini(float martiniHP, int oliveHP, float oliveSpawnDelay, string moveString, float oliveSpeed, float oliveStopDuration, string[] olivePositionStringY, string[] olivePositionStringX, float bulletSpeed, string pinkString, float oliveHesitateAfterShooting)
			{
				this.martiniHP = martiniHP;
				this.oliveHP = oliveHP;
				this.oliveSpawnDelay = oliveSpawnDelay;
				this.moveString = moveString;
				this.oliveSpeed = oliveSpeed;
				this.oliveStopDuration = oliveStopDuration;
				this.olivePositionStringY = olivePositionStringY;
				this.olivePositionStringX = olivePositionStringX;
				this.bulletSpeed = bulletSpeed;
				this.pinkString = pinkString;
				this.oliveHesitateAfterShooting = oliveHesitateAfterShooting;
			}
		}

		public class Main : AbstractLevelPropertyGroup
		{
			public readonly float delaySubstractAmount;

			public Main(float delaySubstractAmount)
			{
				this.delaySubstractAmount = delaySubstractAmount;
			}
		}

		public DicePalaceBooze(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 500f;
				break;
			case Level.Mode.Hard:
				timeline.health = 500f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DicePalaceBooze.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DicePalaceBooze GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Decanter(0f, 0f, string.Empty, new MinMax(0f, 1f)), new Tumbler(0f, string.Empty, 0f, 0f), new Martini(0f, 0, 0f, string.Empty, 0f, 0f, new string[0], new string[0], 0f, string.Empty, 0f), new Main(0f)));
				break;
			case Level.Mode.Normal:
				hp = 500;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Decanter(215f, 730f, "2.9,3.5,4.2,3.4,2.8,4.2,3,3.4", new MinMax(0.9f, 1.2f)), new Tumbler(215f, "4.6,5,5.4,5.8,4.1,5.5,5.6", 0.5f, 0.75f), new Martini(215f, 8, 3.6f, "3,5,4,3,3,4", 500f, 1f, new string[2] { "400,650,375,500", "600,475,550,450" }, new string[2] { "0,500,700,300,725,150,500", "100,400,600,250,450" }, 350f, "1,2", 1f), new Main(0.3f)));
				break;
			case Level.Mode.Hard:
				hp = 500;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Decanter(265f, 730f, "2.9,3.5,4.2,3.4,2.8,4.2,3,3.4", new MinMax(0.8f, 1.3f)), new Tumbler(265f, "4.6,5,4.4,5.1,4.1,5.5,5.6", 0.5f, 0.75f), new Martini(265f, 8, 3.6f, "3,5,4,3,3,4", 530f, 1f, new string[3] { "400,650,375,525", "425,500,625", "600,475,550,450" }, new string[3] { "0,500,700,300,725,150,500", "550,200,650,50", "100,400,600,250,450" }, 350f, "1,2", 1f), new Main(0.3f)));
				break;
			}
			return new DicePalaceBooze(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceCard : AbstractLevelProperties<DicePalaceCard.State, DicePalaceCard.Pattern, DicePalaceCard.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceCard properties { get; private set; }

			public virtual void LevelInit(DicePalaceCard properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Blocks blocks;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Blocks blocks)
				: base(healthTrigger, patterns, stateName)
			{
				this.blocks = blocks;
			}
		}

		public class Blocks : AbstractLevelPropertyGroup
		{
			public readonly float blockSpeed;

			public readonly float blockDropSpeed;

			public readonly string[] cardTypeString;

			public readonly string[] cardAmountString;

			public readonly float attackDelayRange;

			public readonly int gridWidth;

			public readonly int gridHeight;

			public readonly float blockSize;

			public Blocks(float blockSpeed, float blockDropSpeed, string[] cardTypeString, string[] cardAmountString, float attackDelayRange, int gridWidth, int gridHeight, float blockSize)
			{
				this.blockSpeed = blockSpeed;
				this.blockDropSpeed = blockDropSpeed;
				this.cardTypeString = cardTypeString;
				this.cardAmountString = cardAmountString;
				this.attackDelayRange = attackDelayRange;
				this.gridWidth = gridWidth;
				this.gridHeight = gridHeight;
				this.blockSize = blockSize;
			}
		}

		public DicePalaceCard(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 100f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DicePalaceCard.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DicePalaceCard GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Blocks(0f, 0f, new string[0], new string[0], 0f, 0, 0, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Blocks(550f, 400f, new string[1] { "H,H,S,S,C,H,S,H,C,S" }, new string[1] { "3,3" }, 2.5f, 8, 7, 70f)));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Blocks(0f, 0f, new string[0], new string[0], 0f, 0, 0, 0f)));
				break;
			}
			return new DicePalaceCard(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceChips : AbstractLevelProperties<DicePalaceChips.State, DicePalaceChips.Pattern, DicePalaceChips.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceChips properties { get; private set; }

			public virtual void LevelInit(DicePalaceChips properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Chips chips;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Chips chips)
				: base(healthTrigger, patterns, stateName)
			{
				this.chips = chips;
			}
		}

		public class Chips : AbstractLevelPropertyGroup
		{
			public readonly float initialAttackDelay;

			public readonly float chipSpeedMultiplier;

			public readonly string[] chipAttackString;

			public readonly float chipAttackDelay;

			public readonly MinMax attackCycleDelay;

			public readonly float chipSpacing;

			public Chips(float initialAttackDelay, float chipSpeedMultiplier, string[] chipAttackString, float chipAttackDelay, MinMax attackCycleDelay, float chipSpacing)
			{
				this.initialAttackDelay = initialAttackDelay;
				this.chipSpeedMultiplier = chipSpeedMultiplier;
				this.chipAttackString = chipAttackString;
				this.chipAttackDelay = chipAttackDelay;
				this.attackCycleDelay = attackCycleDelay;
				this.chipSpacing = chipSpacing;
			}
		}

		public DicePalaceChips(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 450f;
				break;
			case Level.Mode.Hard:
				timeline.health = 575f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DicePalaceChips.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DicePalaceChips GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Chips(0f, 0f, new string[0], 0f, new MinMax(0f, 1f), 0f)));
				break;
			case Level.Mode.Normal:
				hp = 450;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Chips(3f, 0.8f, new string[14]
				{
					"1-2-3-4,5-6-7-8", "1-8,2-7,3-4-5-6", "3-4-5,1-2-6-7-8", "5-6-7-8,1-2-3-4", "5-6-7,1-2-8,3-4", "1-2-3-4-5-6,7-8", "1-2-8,7-6,3-4-5", "2-3-4-5,1-6-7-8", "8-5,2-3-4,1-6-7", "1-8,2-3-4-5,6-7",
					"3-4-5,1-6-7,2-8", "6-7-8,3-4-5,1-2", "5-6-7,1-8,2-3-4", "1-2-7-8,3-4-5-6"
				}, 1f, new MinMax(2f, 3f), 1.2f)));
				break;
			case Level.Mode.Hard:
				hp = 575;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Chips(3f, 0.8f, new string[17]
				{
					"1-2-8,3-4-5,6-7", "1-2,3-8,4-5-6-7", "1-2-3-4,5-6-7-8", "2-4-6-1,3-8,5-7", "1-7-8,2-3,4-5-6", "2-3-8,1-5-6,4-7", "3-4-5-6,1-2-7-8", "4-5-6-7,1-2-3-8", "5-6-7-8,1-2-3-4", "2-3-8-1,4-5-6-7",
					"3-4-5,1-2-6-7-8", "1-8,3-4-5,2-6-7", "4-5-6,1-2-3-7-8", "5-6-7-8,1-2-3-4", "2-3-4,1-5-6-7-8", "3-4-5-6,1-8,2-7", "1-2-3-8,4-5-6-7"
				}, 0.8f, new MinMax(1.5f, 2.5f), 1.2f)));
				break;
			}
			return new DicePalaceChips(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceCigar : AbstractLevelProperties<DicePalaceCigar.State, DicePalaceCigar.Pattern, DicePalaceCigar.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceCigar properties { get; private set; }

			public virtual void LevelInit(DicePalaceCigar properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Cigar cigar;

			public readonly SpiralSmoke spiralSmoke;

			public readonly CigaretteGhost cigaretteGhost;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Cigar cigar, SpiralSmoke spiralSmoke, CigaretteGhost cigaretteGhost)
				: base(healthTrigger, patterns, stateName)
			{
				this.cigar = cigar;
				this.spiralSmoke = spiralSmoke;
				this.cigaretteGhost = cigaretteGhost;
			}
		}

		public class Cigar : AbstractLevelPropertyGroup
		{
			public readonly float warningDelay;

			public readonly float platformWidthMultiplier;

			public readonly float platformHeight;

			public Cigar(float warningDelay, float platformWidthMultiplier, float platformHeight)
			{
				this.warningDelay = warningDelay;
				this.platformWidthMultiplier = platformWidthMultiplier;
				this.platformHeight = platformHeight;
			}
		}

		public class SpiralSmoke : AbstractLevelPropertyGroup
		{
			public readonly float horizontalSpeed;

			public readonly float circleSpeed;

			public readonly string rotationDirectionString;

			public readonly float attackDelay;

			public readonly string attackCount;

			public readonly float spiralSmokeCircleSize;

			public readonly float hesitateBeforeAttackDelay;

			public SpiralSmoke(float horizontalSpeed, float circleSpeed, string rotationDirectionString, float attackDelay, string attackCount, float spiralSmokeCircleSize, float hesitateBeforeAttackDelay)
			{
				this.horizontalSpeed = horizontalSpeed;
				this.circleSpeed = circleSpeed;
				this.rotationDirectionString = rotationDirectionString;
				this.attackDelay = attackDelay;
				this.attackCount = attackCount;
				this.spiralSmokeCircleSize = spiralSmokeCircleSize;
				this.hesitateBeforeAttackDelay = hesitateBeforeAttackDelay;
			}
		}

		public class CigaretteGhost : AbstractLevelPropertyGroup
		{
			public readonly float verticalSpeed;

			public readonly float horizontalSpeed;

			public readonly string attackDelayString;

			public readonly float horizontalSpacing;

			public readonly string spawnPositionString;

			public CigaretteGhost(float verticalSpeed, float horizontalSpeed, string attackDelayString, float horizontalSpacing, string spawnPositionString)
			{
				this.verticalSpeed = verticalSpeed;
				this.horizontalSpeed = horizontalSpeed;
				this.attackDelayString = attackDelayString;
				this.horizontalSpacing = horizontalSpacing;
				this.spawnPositionString = spawnPositionString;
			}
		}

		public DicePalaceCigar(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 700f;
				break;
			case Level.Mode.Hard:
				timeline.health = 850f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DicePalaceCigar.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DicePalaceCigar GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Cigar(0f, 0f, 0f), new SpiralSmoke(0f, 0f, string.Empty, 0f, string.Empty, 0f, 0f), new CigaretteGhost(0f, 0f, string.Empty, 0f, string.Empty)));
				break;
			case Level.Mode.Normal:
				hp = 700;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Cigar(1.4f, 1f, 150f), new SpiralSmoke(145f, 2.8f, "1,1,2,1,1,2,2,2", 3.3f, "0,1,0,1,1,2,0,1", 160f, 1f), new CigaretteGhost(220f, 3.1f, "2,2.5,2.6,2.2,2.7,2.1,2,2.5", 130f, "0,50,-50,0,25,-25")));
				break;
			case Level.Mode.Hard:
				hp = 850;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Cigar(0.55f, 1f, 150f), new SpiralSmoke(185f, 3.6f, "2,2,1,2,2,1,1,1", 1.4f, "1,0,2,1,2,0,2,2,1", 205f, 0.5f), new CigaretteGhost(220f, 2f, "1.8,2.1,1.7,1.9,2.3,1.7,1.7,2.4,1.5,1.9,2.2", 90f, "0,50,-50,0,25,-25")));
				break;
			}
			return new DicePalaceCigar(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceDomino : AbstractLevelProperties<DicePalaceDomino.State, DicePalaceDomino.Pattern, DicePalaceDomino.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceDomino properties { get; private set; }

			public virtual void LevelInit(DicePalaceDomino properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Boomerang = 0,
			BouncyBall = 1,
			Uninitialized = 2
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Domino domino;

			public readonly BouncyBall bouncyBall;

			public readonly Boomerang boomerang;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Domino domino, BouncyBall bouncyBall, Boomerang boomerang)
				: base(healthTrigger, patterns, stateName)
			{
				this.domino = domino;
				this.bouncyBall = bouncyBall;
				this.boomerang = boomerang;
			}
		}

		public class Domino : AbstractLevelPropertyGroup
		{
			public readonly int dominoHP;

			public readonly float swingSpeed;

			public readonly float swingDistance;

			public readonly float swingPosY;

			public readonly float floorSpeed;

			public readonly string floorColourString;

			public readonly float floorTileScale;

			public readonly float spikesWarningDuration;

			public readonly string mainString;

			public Domino(int dominoHP, float swingSpeed, float swingDistance, float swingPosY, float floorSpeed, string floorColourString, float floorTileScale, float spikesWarningDuration, string mainString)
			{
				this.dominoHP = dominoHP;
				this.swingSpeed = swingSpeed;
				this.swingDistance = swingDistance;
				this.swingPosY = swingPosY;
				this.floorSpeed = floorSpeed;
				this.floorColourString = floorColourString;
				this.floorTileScale = floorTileScale;
				this.spikesWarningDuration = spikesWarningDuration;
				this.mainString = mainString;
			}
		}

		public class BouncyBall : AbstractLevelPropertyGroup
		{
			public readonly float bulletSpeed;

			public readonly string angleString;

			public readonly string upDownString;

			public readonly MinMax attackDelayRange;

			public readonly string projectileTypeString;

			public readonly float initialAttackDelay;

			public BouncyBall(float bulletSpeed, string angleString, string upDownString, MinMax attackDelayRange, string projectileTypeString, float initialAttackDelay)
			{
				this.bulletSpeed = bulletSpeed;
				this.angleString = angleString;
				this.upDownString = upDownString;
				this.attackDelayRange = attackDelayRange;
				this.projectileTypeString = projectileTypeString;
				this.initialAttackDelay = initialAttackDelay;
			}
		}

		public class Boomerang : AbstractLevelPropertyGroup
		{
			public readonly float boomerangSpeed;

			public readonly MinMax attackDelayRange;

			public readonly string boomerangTypeString;

			public readonly float initialAttackDelay;

			public readonly float health;

			public Boomerang(float boomerangSpeed, MinMax attackDelayRange, string boomerangTypeString, float initialAttackDelay, float health)
			{
				this.boomerangSpeed = boomerangSpeed;
				this.attackDelayRange = attackDelayRange;
				this.boomerangTypeString = boomerangTypeString;
				this.initialAttackDelay = initialAttackDelay;
				this.health = health;
			}
		}

		public DicePalaceDomino(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 600f;
				break;
			case Level.Mode.Hard:
				timeline.health = 750f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "B":
				return Pattern.Boomerang;
			case "S":
				return Pattern.BouncyBall;
			default:
				Debug.LogError("Pattern DicePalaceDomino.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static DicePalaceDomino GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[7]
				{
					Pattern.BouncyBall,
					Pattern.BouncyBall,
					Pattern.Boomerang,
					Pattern.BouncyBall,
					Pattern.BouncyBall,
					Pattern.BouncyBall,
					Pattern.Boomerang
				} }, States.Main, new Domino(0, 0f, 0f, 0f, 0f, string.Empty, 0f, 0f, string.Empty), new BouncyBall(0f, string.Empty, string.Empty, new MinMax(0f, 1f), "R,R,R,P,R,R,R,P", 0f), new Boomerang(0f, new MinMax(0f, 1f), string.Empty, 0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 600;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[7]
				{
					Pattern.BouncyBall,
					Pattern.BouncyBall,
					Pattern.Boomerang,
					Pattern.BouncyBall,
					Pattern.BouncyBall,
					Pattern.BouncyBall,
					Pattern.Boomerang
				} }, States.Main, new Domino(500, 1.3f, 80f, 440f, 270f, "R,G,B,Y,R,R,G,B,Y,R,G,B,B,Y,R,G,G,B,Y,R,G,B,Y,Y", 1f, 0.8f, "B,B,B,S,S,B"), new BouncyBall(575f, "70,72,68,66,68,65,70,67,65,69,66", "U,U,D,U,D,D,D,U,D,U,U,D,U,D,D", new MinMax(1f, 1.5f), "R,R,P,R,P,R,R,P", 2f), new Boomerang(350f, new MinMax(1f, 1.5f), "R", 2f, 20f)));
				break;
			case Level.Mode.Hard:
				hp = 750;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[7]
				{
					Pattern.BouncyBall,
					Pattern.BouncyBall,
					Pattern.Boomerang,
					Pattern.BouncyBall,
					Pattern.BouncyBall,
					Pattern.BouncyBall,
					Pattern.Boomerang
				} }, States.Main, new Domino(500, 1.3f, 80f, 440f, 345f, "R,G,B,Y,R,R,G,B,Y,R,G,B,B,Y,R,G,G,B,Y,R,G,B,Y,Y", 1f, 2f, "B,B,B,S,S,B"), new BouncyBall(650f, "71,76,72,70,75,76,73,75,72,74,69", "D,U,U,U,D,D,U,D,U,D,D,U,U", new MinMax(0.1f, 0.5f), "R,R,R,P,R,R,P", 1f), new Boomerang(400f, new MinMax(0.1f, 0.5f), "R", 1f, 20f)));
				break;
			}
			return new DicePalaceDomino(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceEightBall : AbstractLevelProperties<DicePalaceEightBall.State, DicePalaceEightBall.Pattern, DicePalaceEightBall.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceEightBall properties { get; private set; }

			public virtual void LevelInit(DicePalaceEightBall properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly General general;

			public readonly PoolBalls poolBalls;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, General general, PoolBalls poolBalls)
				: base(healthTrigger, patterns, stateName)
			{
				this.general = general;
				this.poolBalls = poolBalls;
			}
		}

		public class General : AbstractLevelPropertyGroup
		{
			public readonly float shootSpeed;

			public readonly string[] shootString;

			public readonly float shootDelay;

			public readonly int idleLoopAmount;

			public readonly float attackDuration;

			public General(float shootSpeed, string[] shootString, float shootDelay, int idleLoopAmount, float attackDuration)
			{
				this.shootSpeed = shootSpeed;
				this.shootString = shootString;
				this.shootDelay = shootDelay;
				this.idleLoopAmount = idleLoopAmount;
				this.attackDuration = attackDuration;
			}
		}

		public class PoolBalls : AbstractLevelPropertyGroup
		{
			public readonly string[] sideString;

			public readonly float spawnDelay;

			public readonly float oneGroundDelay;

			public readonly float oneJumpVerticalSpeed;

			public readonly float oneJumpHorizontalSpeed;

			public readonly float oneJumpGravity;

			public readonly float twoGroundDelay;

			public readonly float twoJumpVerticalSpeed;

			public readonly float twoJumpHorizontalSpeed;

			public readonly float twoJumpGravity;

			public readonly float threeGroundDelay;

			public readonly float threeJumpVerticalSpeed;

			public readonly float threeJumpHorizontalSpeed;

			public readonly float threeJumpGravity;

			public readonly float fourGroundDelay;

			public readonly float fourJumpVerticalSpeed;

			public readonly float fourJumpHorizontalSpeed;

			public readonly float fourJumpGravity;

			public readonly float fiveGroundDelay;

			public readonly float fiveJumpVerticalSpeed;

			public readonly float fiveJumpHorizontalSpeed;

			public readonly float fiveJumpGravity;

			public PoolBalls(string[] sideString, float spawnDelay, float oneGroundDelay, float oneJumpVerticalSpeed, float oneJumpHorizontalSpeed, float oneJumpGravity, float twoGroundDelay, float twoJumpVerticalSpeed, float twoJumpHorizontalSpeed, float twoJumpGravity, float threeGroundDelay, float threeJumpVerticalSpeed, float threeJumpHorizontalSpeed, float threeJumpGravity, float fourGroundDelay, float fourJumpVerticalSpeed, float fourJumpHorizontalSpeed, float fourJumpGravity, float fiveGroundDelay, float fiveJumpVerticalSpeed, float fiveJumpHorizontalSpeed, float fiveJumpGravity)
			{
				this.sideString = sideString;
				this.spawnDelay = spawnDelay;
				this.oneGroundDelay = oneGroundDelay;
				this.oneJumpVerticalSpeed = oneJumpVerticalSpeed;
				this.oneJumpHorizontalSpeed = oneJumpHorizontalSpeed;
				this.oneJumpGravity = oneJumpGravity;
				this.twoGroundDelay = twoGroundDelay;
				this.twoJumpVerticalSpeed = twoJumpVerticalSpeed;
				this.twoJumpHorizontalSpeed = twoJumpHorizontalSpeed;
				this.twoJumpGravity = twoJumpGravity;
				this.threeGroundDelay = threeGroundDelay;
				this.threeJumpVerticalSpeed = threeJumpVerticalSpeed;
				this.threeJumpHorizontalSpeed = threeJumpHorizontalSpeed;
				this.threeJumpGravity = threeJumpGravity;
				this.fourGroundDelay = fourGroundDelay;
				this.fourJumpVerticalSpeed = fourJumpVerticalSpeed;
				this.fourJumpHorizontalSpeed = fourJumpHorizontalSpeed;
				this.fourJumpGravity = fourJumpGravity;
				this.fiveGroundDelay = fiveGroundDelay;
				this.fiveJumpVerticalSpeed = fiveJumpVerticalSpeed;
				this.fiveJumpHorizontalSpeed = fiveJumpHorizontalSpeed;
				this.fiveJumpGravity = fiveJumpGravity;
			}
		}

		public DicePalaceEightBall(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 600f;
				break;
			case Level.Mode.Hard:
				timeline.health = 750f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DicePalaceEightBall.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DicePalaceEightBall GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new General(0f, new string[0], 0f, 0, 0f), new PoolBalls(new string[0], 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 600;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new General(550f, new string[1] { "R,R" }, 3.3f, 2, 2f), new PoolBalls(new string[1] { "L,R" }, 3.8f, 0.5f, 2000f, 580f, 7000f, 0.7f, 2200f, 780f, 7000f, 0.6f, 2100f, 680f, 7000f, 0.4f, 2200f, 400f, 7000f, 0.6f, 2100f, 800f, 7000f)));
				break;
			case Level.Mode.Hard:
				hp = 750;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new General(550f, new string[1] { "R,R" }, 3.3f, 2, 2f), new PoolBalls(new string[1] { "L,R" }, 2.35f, 0.1f, 2000f, 600f, 7000f, 0.7f, 2300f, 800f, 7000f, 0.5f, 1900f, 900f, 7000f, 0.4f, 2200f, 400f, 7000f, 0.6f, 2100f, 800f, 7000f)));
				break;
			}
			return new DicePalaceEightBall(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceFlyingHorse : AbstractLevelProperties<DicePalaceFlyingHorse.State, DicePalaceFlyingHorse.Pattern, DicePalaceFlyingHorse.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceFlyingHorse properties { get; private set; }

			public virtual void LevelInit(DicePalaceFlyingHorse properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly GiftBombs giftBombs;

			public readonly MiniHorses miniHorses;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, GiftBombs giftBombs, MiniHorses miniHorses)
				: base(healthTrigger, patterns, stateName)
			{
				this.giftBombs = giftBombs;
				this.miniHorses = miniHorses;
			}
		}

		public class GiftBombs : AbstractLevelPropertyGroup
		{
			public readonly float initialSpeed;

			public readonly float explosionSpeed;

			public readonly float explosionTime;

			public readonly MinMax playerAimRange;

			public readonly string[] giftPositionStringY;

			public readonly string[] giftPositionStringX;

			public readonly string spreadCount;

			public readonly float giftDelay;

			public GiftBombs(float initialSpeed, float explosionSpeed, float explosionTime, MinMax playerAimRange, string[] giftPositionStringY, string[] giftPositionStringX, string spreadCount, float giftDelay)
			{
				this.initialSpeed = initialSpeed;
				this.explosionSpeed = explosionSpeed;
				this.explosionTime = explosionTime;
				this.playerAimRange = playerAimRange;
				this.giftPositionStringY = giftPositionStringY;
				this.giftPositionStringX = giftPositionStringX;
				this.spreadCount = spreadCount;
				this.giftDelay = giftDelay;
			}
		}

		public class MiniHorses : AbstractLevelPropertyGroup
		{
			public readonly float HP;

			public readonly string[] delayString;

			public readonly MinMax miniSpeedRange;

			public readonly string[] miniTypeString;

			public readonly float miniTwoBulletSpeed;

			public readonly float miniThreeJockeySpeed;

			public readonly MinMax miniTwoShotDelayRange;

			public readonly string[] miniThreeProxString;

			public readonly string[] miniTwoPinkString;

			public MiniHorses(float HP, string[] delayString, MinMax miniSpeedRange, string[] miniTypeString, float miniTwoBulletSpeed, float miniThreeJockeySpeed, MinMax miniTwoShotDelayRange, string[] miniThreeProxString, string[] miniTwoPinkString)
			{
				this.HP = HP;
				this.delayString = delayString;
				this.miniSpeedRange = miniSpeedRange;
				this.miniTypeString = miniTypeString;
				this.miniTwoBulletSpeed = miniTwoBulletSpeed;
				this.miniThreeJockeySpeed = miniThreeJockeySpeed;
				this.miniTwoShotDelayRange = miniTwoShotDelayRange;
				this.miniThreeProxString = miniThreeProxString;
				this.miniTwoPinkString = miniTwoPinkString;
			}
		}

		public DicePalaceFlyingHorse(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 1000f;
				break;
			case Level.Mode.Hard:
				timeline.health = 1200f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DicePalaceFlyingHorse.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DicePalaceFlyingHorse GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new GiftBombs(0f, 0f, 0f, new MinMax(0f, 1f), new string[0], new string[0], string.Empty, 0f), new MiniHorses(0f, new string[0], new MinMax(0f, 1f), new string[0], 0f, 0f, new MinMax(0f, 1f), new string[0], new string[0])));
				break;
			case Level.Mode.Normal:
				hp = 1000;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new GiftBombs(300f, 200f, 1f, new MinMax(2f, 5f), new string[1] { "100,360,245,160,360,200,100,345,150,275,360,100,255,180" }, new string[1] { "100,600,350,477,100,285,600,100,550,300,100,480,600" }, "0,45,90,135,180,225,270,315", 3f), new MiniHorses(999f, new string[2] { "1,1.4,1.6,1,1.8,1.4,1,1,1.5,1.2,1,1.3,1.4", "1,1,1.5,1.5,1.2,1.3,1,1.8,1.6,1.4,1.6,1.3" }, new MinMax(270f, 450f), new string[2] { "1,2,1,3,1,2,1,2,3,1,2,2,1,3,1,2,1,2,1,3", "1,2,1,2,1,3,1,2,3,1,2,1,2,2,3,2,1,2,1,3" }, 400f, 600f, new MinMax(999f, 999f), new string[1] { "0,100,50,0,120,0,-50,0,80,-20,0,50" }, new string[1] { "1,1" })));
				break;
			case Level.Mode.Hard:
				hp = 1200;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new GiftBombs(350f, 200f, 1f, new MinMax(2f, 5f), new string[1] { "100,360,245,160,360,200,100,345,150,275,360,100,255,180" }, new string[1] { "100,600,350,477,100,285,600,100,550,300,100,480,600" }, "0,45,90,135,180,225,270,315", 1.5f), new MiniHorses(9999f, new string[2] { "1,1.3,1.4,1,1.5,1.2,1.2,1.3,1.7,1.4,1.3,1.6", "1,1.2,1.5,1,,1.4,1.6,1,1.1,1.3,1.4,1.6,1.2,1" }, new MinMax(270f, 450f), new string[2] { "1,2,1,3,1,2,1,1,3,1,2,3,1,2,3,1,2,2,3,2,2,1,1,3,1,1,2,1,3", "1,2,3,1,2,2,1,3,1,2,3,1,2,1,1,3,1,2,3,1,2,2,1,3,1,2,2,3" }, 400f, 600f, new MinMax(999f, 999f), new string[1] { "0,100,50,0,120,0,-50,0,80,-20,0,50" }, new string[1] { "1,1" })));
				break;
			}
			return new DicePalaceFlyingHorse(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceFlyingMemory : AbstractLevelProperties<DicePalaceFlyingMemory.State, DicePalaceFlyingMemory.Pattern, DicePalaceFlyingMemory.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceFlyingMemory properties { get; private set; }

			public virtual void LevelInit(DicePalaceFlyingMemory properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Bots bots;

			public readonly FlippyCard flippyCard;

			public readonly StuffedToy stuffedToy;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Bots bots, FlippyCard flippyCard, StuffedToy stuffedToy)
				: base(healthTrigger, patterns, stateName)
			{
				this.bots = bots;
				this.flippyCard = flippyCard;
				this.stuffedToy = stuffedToy;
			}
		}

		public class Bots : AbstractLevelPropertyGroup
		{
			public readonly float botsSpeed;

			public readonly float botsScale;

			public readonly float botsHP;

			public readonly float bulletWarningDuration;

			public readonly float bulletSpeed;

			public readonly string[] spawnOrder;

			public readonly float spawnDelay;

			public readonly string[] movementString;

			public readonly string[] directionString;

			public readonly float bulletDelay;

			public readonly bool botsOn;

			public Bots(float botsSpeed, float botsScale, float botsHP, float bulletWarningDuration, float bulletSpeed, string[] spawnOrder, float spawnDelay, string[] movementString, string[] directionString, float bulletDelay, bool botsOn)
			{
				this.botsSpeed = botsSpeed;
				this.botsScale = botsScale;
				this.botsHP = botsHP;
				this.bulletWarningDuration = bulletWarningDuration;
				this.bulletSpeed = bulletSpeed;
				this.spawnOrder = spawnOrder;
				this.spawnDelay = spawnDelay;
				this.movementString = movementString;
				this.directionString = directionString;
				this.bulletDelay = bulletDelay;
				this.botsOn = botsOn;
			}
		}

		public class FlippyCard : AbstractLevelPropertyGroup
		{
			public readonly string[] patternOrder;

			public readonly float initialRevealTime;

			public FlippyCard(string[] patternOrder, float initialRevealTime)
			{
				this.patternOrder = patternOrder;
				this.initialRevealTime = initialRevealTime;
			}
		}

		public class StuffedToy : AbstractLevelPropertyGroup
		{
			public readonly string[] angleString;

			public readonly string[] bounceCount;

			public readonly float bounceSpeed;

			public readonly float punishSpeed;

			public readonly float punishTime;

			public readonly float directionChangeDelay;

			public readonly float attackAnti;

			public readonly MinMax shotDelayRange;

			public readonly string[] shotType;

			public readonly float incrementSpeedBy;

			public readonly string[] angleAdditionString;

			public readonly float regularSpeed;

			public readonly float spreadSpeed;

			public readonly MinMax spreadAngle;

			public readonly int spreadBullets;

			public readonly float spiralSpeed;

			public readonly float spiralMovementRate;

			public readonly float musicDeathTimer;

			public StuffedToy(string[] angleString, string[] bounceCount, float bounceSpeed, float punishSpeed, float punishTime, float directionChangeDelay, float attackAnti, MinMax shotDelayRange, string[] shotType, float incrementSpeedBy, string[] angleAdditionString, float regularSpeed, float spreadSpeed, MinMax spreadAngle, int spreadBullets, float spiralSpeed, float spiralMovementRate, float musicDeathTimer)
			{
				this.angleString = angleString;
				this.bounceCount = bounceCount;
				this.bounceSpeed = bounceSpeed;
				this.punishSpeed = punishSpeed;
				this.punishTime = punishTime;
				this.directionChangeDelay = directionChangeDelay;
				this.attackAnti = attackAnti;
				this.shotDelayRange = shotDelayRange;
				this.shotType = shotType;
				this.incrementSpeedBy = incrementSpeedBy;
				this.angleAdditionString = angleAdditionString;
				this.regularSpeed = regularSpeed;
				this.spreadSpeed = spreadSpeed;
				this.spreadAngle = spreadAngle;
				this.spreadBullets = spreadBullets;
				this.spiralSpeed = spiralSpeed;
				this.spiralMovementRate = spiralMovementRate;
				this.musicDeathTimer = musicDeathTimer;
			}
		}

		public DicePalaceFlyingMemory(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 800f;
				break;
			case Level.Mode.Hard:
				timeline.health = 1000f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DicePalaceFlyingMemory.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DicePalaceFlyingMemory GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Bots(0f, 0f, 0f, 0f, 0f, new string[0], 0f, new string[0], new string[0], 0f, false), new FlippyCard(new string[0], 0f), new StuffedToy(new string[0], new string[0], 0f, 0f, 0f, 0f, 0f, new MinMax(0f, 1f), new string[0], 0f, new string[0], 0f, 0f, new MinMax(0f, 1f), 0, 0f, 0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 800;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Bots(200f, 1f, 10f, 1f, 200f, new string[1] { "U:2,D:4,L:1,R:3,U:4,L:3" }, 9999f, new string[1] { "2,4,1,3" }, new string[1] { "N,N,N,P,N,N,N,N,N,P" }, 5f, false), new FlippyCard(new string[12]
				{
					"1A,3B,2B,3B,1B,3A,1B,2A,1A,2A,3A,2B", "3B,1B,2B,1B,2A,3A,1A,3B,2B,2A,1A,3A", "2B,2A,1A,1A,3B,3A,2B,1B,1B,3B,2A,3A", "3B,2B,1B,1A,2B,3A,1B,2A,1A,2A,3A,3B", "3A,2A,3A,1A,1A,2B,3B,2A,2B,3B,1B,1B", "1B,1A,3B,1B,3A,2A,2B,1A,2B,2A,3A,3B", "2A,3B,1A,1A,2B,3A,3B,1B,1B,3A,2A,2B", "1A,2B,3B,3A,3B,1A,1B,2A,2A,2B,3A,1B", "3A,2A,1A,2B,1A,3B,1B,2A,2B,1B,3B,3A", "2A,1A,1B,3B,1B,3A,2A,2B,1A,3B,2B,3A",
					"1B,3A,3A,3B,2A,2B,1B,1A,3B,1A,2B,2A", "2B,3A,3B,2A,2A,2B,1A,1A,1B,3B,3A,1B"
				}, 2.1f), new StuffedToy(new string[3] { "220,45,125,325,37,140,305,55,135,235,40,140", "215,40,120,320,36,135,300,50,130,230,35,135", "225,50,130,330,42,145,310,60,140,240,44,145" }, new string[1] { "2,4" }, 530f, 730f, 6f, 1.3f, 0.5f, new MinMax(1.8f, 3.3f), new string[1] { "2" }, 5f, new string[1] { "0" }, 500f, 500f, new MinMax(0f, 300f), 6, 100f, 0.6f, 0.4f)));
				break;
			case Level.Mode.Hard:
				hp = 1000;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Bots(1f, 1f, 1f, 1f, 1f, new string[1] { "U:2,D:4,L:1,R:3,U:4,L:3" }, 99999f, new string[1] { "2,4,1,3" }, new string[1] { "N,N,N,P,N,N,N,N,N,P" }, 9999f, false), new FlippyCard(new string[12]
				{
					"2B,3A,3B,2A,2A,2B,1A,1A,1B,3B,3A,1B", "1B,3A,3A,3B,2A,2B,1B,1A,3B,1A,2B,2A", "2A,1A,1B,3B,1B,3A,2A,2B,1A,3B,2B,3A", "3A,2A,1A,2B,1A,3B,1B,2A,2B,1B,3B,3A", "1A,2B,3B,3A,3B,1A,1B,2A,2A,2B,3A,1B", "2A,3B,1A,1A,2B,3A,3B,1B,1B,3A,2A,2B", "1B,1A,3B,1B,3A,2A,2B,1A,2B,2A,3A,3B", "3A,2A,3A,1A,1A,2B,3B,2A,2B,3B,1B,1B", "3B,2B,1B,1A,2B,3A,1B,2A,1A,2A,3A,3B", "2B,2A,1A,1A,3B,3A,2B,1B,1B,3B,2A,3A",
					"3B,1B,2B,1B,2A,3A,1A,3B,2B,2A,1A,3A", "1A,3B,2B,3B,1B,3A,1B,2A,1A,2A,3A,2B"
				}, 2.1f), new StuffedToy(new string[1] { "220,45,125,325,37,140,305,55,135,235,40,140" }, new string[1] { "2,4" }, 600f, 900f, 6f, 1.3f, 1f, new MinMax(1.4f, 2.8f), new string[1] { "2" }, 5f, new string[1] { "5,-3,8,-6,7,-4,8,-6,1,-5,-5" }, 500f, 500f, new MinMax(0f, 300f), 6, 500f, 1f, 0.4f)));
				break;
			}
			return new DicePalaceFlyingMemory(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceLight : AbstractLevelProperties<DicePalaceLight.State, DicePalaceLight.Pattern, DicePalaceLight.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceLight properties { get; private set; }

			public virtual void LevelInit(DicePalaceLight properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly General general;

			public readonly SixWayLaser sixWayLaser;

			public readonly Objects objects;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, General general, SixWayLaser sixWayLaser, Objects objects)
				: base(healthTrigger, patterns, stateName)
			{
				this.general = general;
				this.sixWayLaser = sixWayLaser;
				this.objects = objects;
			}
		}

		public class General : AbstractLevelPropertyGroup
		{
			public readonly float platformOnePosition;

			public readonly float platformTwoPosition;

			public readonly float bossPosition;

			public General(float platformOnePosition, float platformTwoPosition, float bossPosition)
			{
				this.platformOnePosition = platformOnePosition;
				this.platformTwoPosition = platformTwoPosition;
				this.bossPosition = bossPosition;
			}
		}

		public class SixWayLaser : AbstractLevelPropertyGroup
		{
			public readonly MinMax rotationSpeedRange;

			public readonly MinMax attackOffDurationRange;

			public readonly float warningDuration;

			public readonly MinMax attackOnDurationRange;

			public readonly string[] directionAttackString;

			public readonly float directionTime;

			public SixWayLaser(MinMax rotationSpeedRange, MinMax attackOffDurationRange, float warningDuration, MinMax attackOnDurationRange, string[] directionAttackString, float directionTime)
			{
				this.rotationSpeedRange = rotationSpeedRange;
				this.attackOffDurationRange = attackOffDurationRange;
				this.warningDuration = warningDuration;
				this.attackOnDurationRange = attackOnDurationRange;
				this.directionAttackString = directionAttackString;
				this.directionTime = directionTime;
			}
		}

		public class Objects : AbstractLevelPropertyGroup
		{
			public readonly float objectSpeed;

			public readonly float objectStartHeight;

			public Objects(float objectSpeed, float objectStartHeight)
			{
				this.objectSpeed = objectSpeed;
				this.objectStartHeight = objectStartHeight;
			}
		}

		public DicePalaceLight(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 500f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DicePalaceLight.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DicePalaceLight GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new General(0f, 0f, 0f), new SixWayLaser(new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), new string[0], 0f), new Objects(0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 500;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new General(250f, 500f, 10f), new SixWayLaser(new MinMax(25f, 85f), new MinMax(3f, 5.5f), 1.7f, new MinMax(2f, 3.2f), new string[1] { "1,1,2,1,2,2" }, 9000f), new Objects(200f, 400f)));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new General(0f, 0f, 0f), new SixWayLaser(new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), new string[0], 0f), new Objects(0f, 0f)));
				break;
			}
			return new DicePalaceLight(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceMain : AbstractLevelProperties<DicePalaceMain.State, DicePalaceMain.Pattern, DicePalaceMain.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceMain properties { get; private set; }

			public virtual void LevelInit(DicePalaceMain properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Dice dice;

			public readonly Cards cards;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Dice dice, Cards cards)
				: base(healthTrigger, patterns, stateName)
			{
				this.dice = dice;
				this.cards = cards;
			}
		}

		public class Dice : AbstractLevelPropertyGroup
		{
			public readonly float movementSpeed;

			public readonly float diceStartPositionOneX;

			public readonly float diceStartPositionOneY;

			public readonly float diceStartPositionTwoX;

			public readonly float diceStartPositionTwoY;

			public readonly float rollFrameCount;

			public readonly float pauseWhenRolled;

			public readonly float revealDelay;

			public Dice(float movementSpeed, float diceStartPositionOneX, float diceStartPositionOneY, float diceStartPositionTwoX, float diceStartPositionTwoY, float rollFrameCount, float pauseWhenRolled, float revealDelay)
			{
				this.movementSpeed = movementSpeed;
				this.diceStartPositionOneX = diceStartPositionOneX;
				this.diceStartPositionOneY = diceStartPositionOneY;
				this.diceStartPositionTwoX = diceStartPositionTwoX;
				this.diceStartPositionTwoY = diceStartPositionTwoY;
				this.rollFrameCount = rollFrameCount;
				this.pauseWhenRolled = pauseWhenRolled;
				this.revealDelay = revealDelay;
			}
		}

		public class Cards : AbstractLevelPropertyGroup
		{
			public readonly float cardSpeed;

			public readonly string[] cardString;

			public readonly string[] cardSideOrder;

			public readonly float hesitate;

			public readonly float cardScale;

			public readonly float cardDelay;

			public Cards(float cardSpeed, string[] cardString, string[] cardSideOrder, float hesitate, float cardScale, float cardDelay)
			{
				this.cardSpeed = cardSpeed;
				this.cardString = cardString;
				this.cardSideOrder = cardSideOrder;
				this.hesitate = hesitate;
				this.cardScale = cardScale;
				this.cardDelay = cardDelay;
			}
		}

		public DicePalaceMain(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 600f;
				break;
			case Level.Mode.Hard:
				timeline.health = 750f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DicePalaceMain.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DicePalaceMain GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Dice(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f), new Cards(0f, new string[0], new string[0], 0f, 0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 600;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Dice(1.5f, -300f, 1000f, 300f, 50f, 8f, 1f, 2f), new Cards(410f, new string[11]
				{
					"R,R,P,R,P,R,R,R,P,R,R,P", "R,R,R,P,R,P,R,R,P,R,R,P", "R,R,P,R,R,P,R,P,R,R,P,R", "R,R,R,P,R,R,P,R,R,P,R,P", "R,P,R,R,R,P,R,P,R,R,P,R", "R,R,P,R,R,R,P,R,P,R,R,P", "R,R,R,P,R,P,R,R,P,R,P,R", "R,P,R,R,P,R,P,R,R,P,R,R", "R,R,R,P,R,R,P,R,R,P,R,P", "R,P,R,R,R,P,R,P,R,R,P,R",
					"R,R,P,R,R,P,R,R,R,P,R,R"
				}, new string[1] { "L,R,L,R,L,L,R,L,R,L,R,R" }, 3f, 0.5f, 0.3f)));
				break;
			case Level.Mode.Hard:
				hp = 750;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Dice(1.5f, -300f, 1000f, 300f, 50f, 8f, 1f, 2f), new Cards(410f, new string[13]
				{
					"R,P,R,R,R,P,R,P,R,R,R,R,P,R,R,P", "R,R,P,R,R,R,R,P,R,P,R,R,R,P,R,R", "R,R,R,P,R,R,R,P,R,R,R,P,R,P,R,R", "R,R,P,R,R,P,R,P,R,R,R,R,P,R,R,P", "R,P,R,R,P,R,R,R,P,R,R,P,R,R,R,R", "R,R,R,P,R,P,R,R,R,P,R,R,R,P,R,R", "R,P,R,R,R,R,P,R,R,R,P,R,R,P,R,P", "R,R,P,R,P,R,R,R,P,R,R,R,R,P,R,R", "R,P,R,P,R,R,R,R,P,R,R,P,R,R,R,P", "R,R,R,P,R,P,R,R,P,R,P,R,R,P,R,R",
					"R,R,P,R,R,P,R,R,P,R,R,R,R,P,R,R", "R,R,P,R,P,R,R,R,P,R,R,R,R,P,R,P", "R,R,P,R,R,P,R,P,R,R,R,P,R,R,R,R"
				}, new string[1] { "L,R,L,R,L,L,R,L,R,L,R,R" }, 3f, 0.5f, 0.3f)));
				break;
			}
			return new DicePalaceMain(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalacePachinko : AbstractLevelProperties<DicePalacePachinko.State, DicePalacePachinko.Pattern, DicePalacePachinko.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalacePachinko properties { get; private set; }

			public virtual void LevelInit(DicePalacePachinko properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Pachinko pachinko;

			public readonly Balls balls;

			public readonly Boss boss;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Pachinko pachinko, Balls balls, Boss boss)
				: base(healthTrigger, patterns, stateName)
			{
				this.pachinko = pachinko;
				this.balls = balls;
				this.boss = boss;
			}
		}

		public class Pachinko : AbstractLevelPropertyGroup
		{
			public readonly float platformWidthFour;

			public readonly float platformWidthThree;

			public readonly string platformHeights;

			public Pachinko(float platformWidthFour, float platformWidthThree, string platformHeights)
			{
				this.platformWidthFour = platformWidthFour;
				this.platformWidthThree = platformWidthThree;
				this.platformHeights = platformHeights;
			}
		}

		public class Balls : AbstractLevelPropertyGroup
		{
			public readonly float movementSpeed;

			public readonly string directionString;

			public readonly string spawnOrderString;

			public readonly string ballDelayString;

			public readonly string pinkString;

			public readonly float initialAttackDelay;

			public Balls(float movementSpeed, string directionString, string spawnOrderString, string ballDelayString, string pinkString, float initialAttackDelay)
			{
				this.movementSpeed = movementSpeed;
				this.directionString = directionString;
				this.spawnOrderString = spawnOrderString;
				this.ballDelayString = ballDelayString;
				this.pinkString = pinkString;
				this.initialAttackDelay = initialAttackDelay;
			}
		}

		public class Boss : AbstractLevelPropertyGroup
		{
			public readonly MinMax movementSpeed;

			public readonly MinMax attackDelay;

			public readonly float warningDuration;

			public readonly float beamDuration;

			public readonly float initialAttackDelay;

			public readonly float leftBoundaryOffset;

			public readonly float rightBoundaryOffset;

			public Boss(MinMax movementSpeed, MinMax attackDelay, float warningDuration, float beamDuration, float initialAttackDelay, float leftBoundaryOffset, float rightBoundaryOffset)
			{
				this.movementSpeed = movementSpeed;
				this.attackDelay = attackDelay;
				this.warningDuration = warningDuration;
				this.beamDuration = beamDuration;
				this.initialAttackDelay = initialAttackDelay;
				this.leftBoundaryOffset = leftBoundaryOffset;
				this.rightBoundaryOffset = rightBoundaryOffset;
			}
		}

		public DicePalacePachinko(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 550f;
				break;
			case Level.Mode.Hard:
				timeline.health = 700f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DicePalacePachinko.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DicePalacePachinko GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Pachinko(0f, 0f, string.Empty), new Balls(0f, string.Empty, string.Empty, string.Empty, string.Empty, 0f), new Boss(new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, 0f, 0f, 0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 550;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Pachinko(2f, 2.5f, "150,350,550"), new Balls(365f, "L,L,R,P,R,P,L,R,R,P,R,L,R,P,R,P,L,L,R,P", "1,2,3,2,1,2,3,1,3,2,1,2,3,2,3,1,2,3,1,3", "2.5,2,2.6,2.4,2.1,2.7,2.3,2.8,2.6,2.1,3.1", "4,2,3,4,3,3,2,4,3", 1f), new Boss(new MinMax(300f, 650f), new MinMax(2.8f, 4.8f), 0.75f, 0.9f, 2.4f, -65f, -65f)));
				break;
			case Level.Mode.Hard:
				hp = 700;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Pachinko(2f, 2.5f, "150,350,550"), new Balls(415f, "L,L,R,P,R,P,L,R,R,P,R,L,R,P,R,P,L,L,R,P", "1,2,3,2,1,2,3,1,3,2,1,2,3,2,3,1,2,3,1,3", "2.2,2.4,2.6,2,2.5,2,2.7,2.3,2.3,2.9,2", "4,3,4,3,4,2,3", 1f), new Boss(new MinMax(470f, 730f), new MinMax(2.5f, 4.1f), 0.75f, 1f, 2.4f, -65f, -65f)));
				break;
			}
			return new DicePalacePachinko(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceRabbit : AbstractLevelProperties<DicePalaceRabbit.State, DicePalaceRabbit.Pattern, DicePalaceRabbit.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceRabbit properties { get; private set; }

			public virtual void LevelInit(DicePalaceRabbit properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			MagicWand = 0,
			MagicParry = 1,
			Uninitialized = 2
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly MagicWand magicWand;

			public readonly MagicParry magicParry;

			public readonly General general;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, MagicWand magicWand, MagicParry magicParry, General general)
				: base(healthTrigger, patterns, stateName)
			{
				this.magicWand = magicWand;
				this.magicParry = magicParry;
				this.general = general;
			}
		}

		public class MagicWand : AbstractLevelPropertyGroup
		{
			public readonly float spinningSpeed;

			public readonly float bulletSpeed;

			public readonly MinMax attackDelayRange;

			public readonly float circleDiameter;

			public readonly float hesitate;

			public readonly string safeZoneString;

			public readonly float bulletSize;

			public readonly float initialAttackDelay;

			public MagicWand(float spinningSpeed, float bulletSpeed, MinMax attackDelayRange, float circleDiameter, float hesitate, string safeZoneString, float bulletSize, float initialAttackDelay)
			{
				this.spinningSpeed = spinningSpeed;
				this.bulletSpeed = bulletSpeed;
				this.attackDelayRange = attackDelayRange;
				this.circleDiameter = circleDiameter;
				this.hesitate = hesitate;
				this.safeZoneString = safeZoneString;
				this.bulletSize = bulletSize;
				this.initialAttackDelay = initialAttackDelay;
			}
		}

		public class MagicParry : AbstractLevelPropertyGroup
		{
			public readonly MinMax attackDelayRange;

			public readonly float speed;

			public readonly float hesitate;

			public readonly string pinkString;

			public readonly float initialAttackDelay;

			public readonly string magicPositions;

			public readonly float yOffset;

			public MagicParry(MinMax attackDelayRange, float speed, float hesitate, string pinkString, float initialAttackDelay, string magicPositions, float yOffset)
			{
				this.attackDelayRange = attackDelayRange;
				this.speed = speed;
				this.hesitate = hesitate;
				this.pinkString = pinkString;
				this.initialAttackDelay = initialAttackDelay;
				this.magicPositions = magicPositions;
				this.yOffset = yOffset;
			}
		}

		public class General : AbstractLevelPropertyGroup
		{
			public readonly string platformOnePosition;

			public readonly string platformTwoPosition;

			public General(string platformOnePosition, string platformTwoPosition)
			{
				this.platformOnePosition = platformOnePosition;
				this.platformTwoPosition = platformTwoPosition;
			}
		}

		public DicePalaceRabbit(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 700f;
				break;
			case Level.Mode.Hard:
				timeline.health = 850f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "W":
				return Pattern.MagicWand;
			case "P":
				return Pattern.MagicParry;
			default:
				Debug.LogError("Pattern DicePalaceRabbit.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static DicePalaceRabbit GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main, new MagicWand(0f, 0f, new MinMax(0f, 1f), 0f, 0f, string.Empty, 0f, 0f), new MagicParry(new MinMax(0f, 1f), 0f, 0f, string.Empty, 0f, string.Empty, 0f), new General(string.Empty, string.Empty)));
				break;
			case Level.Mode.Normal:
				hp = 700;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[22]
				{
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicParry
				} }, States.Main, new MagicWand(500f, 175f, new MinMax(1.5f, 2.5f), 530f, 1f, "1,4,7,8,9,6,3,2,4,6,7,2,9,3,8,1,8,7,3,4,2,8,6,1,9,4,3,8,2,1,9,6,8", 2f, 2f), new MagicParry(new MinMax(2f, 2.8f), 405f, 0f, "1-6,5-9,4-7,4-8,2-8,1-5,3-9,2-9,1-5,3-7,4-8,5-9,2-6,4-6,1-7,3-9,2-7,1-5,6-9,2-8,1-4,5-9,1-9,3-7,2-6,1-5,2-8", 1f, "25-135-245-355-465-575-685-795-905", 100f), new General("-200,-50", "1000,1000")));
				break;
			case Level.Mode.Hard:
				hp = 850;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[22]
				{
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicParry,
					Pattern.MagicWand,
					Pattern.MagicWand,
					Pattern.MagicParry
				} }, States.Main, new MagicWand(500f, 245f, new MinMax(1.3f, 2.4f), 530f, 0.4f, "1,4,7,8,9,6,3,2,4,6,7,2,9,3,8,1,8,7,3,4,2,8,6,1,9,4,3,8,2,1,9,6,8", 2f, 2f), new MagicParry(new MinMax(1.5f, 2f), 465f, 0f, "1,8,7,2,9,3,6,1,8,2,9,4,7,2,7,3,9,1,4,1,8,2,6,3,1,7,2,9,4,9,1,7,1,8,3,7,9,4,9,2,8,3,9,1,6,2,8,3,1,7,9,1,6", 1f, "25-135-245-355-465-575-685-795-905", 100f), new General("-200,-50", "1000,1000")));
				break;
			}
			return new DicePalaceRabbit(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceRoulette : AbstractLevelProperties<DicePalaceRoulette.State, DicePalaceRoulette.Pattern, DicePalaceRoulette.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceRoulette properties { get; private set; }

			public virtual void LevelInit(DicePalaceRoulette properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Twirl = 1,
			Marble = 2,
			Uninitialized = 3
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Platform platform;

			public readonly Twirl twirl;

			public readonly MarbleDrop marbleDrop;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Platform platform, Twirl twirl, MarbleDrop marbleDrop)
				: base(healthTrigger, patterns, stateName)
			{
				this.platform = platform;
				this.twirl = twirl;
				this.marbleDrop = marbleDrop;
			}
		}

		public class Platform : AbstractLevelPropertyGroup
		{
			public readonly float platformHeightRow;

			public readonly float platformWidth;

			public readonly float platformCount;

			public readonly float platformOpenDuration;

			public Platform(float platformHeightRow, float platformWidth, float platformCount, float platformOpenDuration)
			{
				this.platformHeightRow = platformHeightRow;
				this.platformWidth = platformWidth;
				this.platformCount = platformCount;
				this.platformOpenDuration = platformOpenDuration;
			}
		}

		public class Twirl : AbstractLevelPropertyGroup
		{
			public readonly float movementSpeed;

			public readonly float moveDelayRange;

			public readonly float hesitate;

			public readonly string[] twirlAmount;

			public readonly float scale;

			public Twirl(float movementSpeed, float moveDelayRange, float hesitate, string[] twirlAmount, float scale)
			{
				this.movementSpeed = movementSpeed;
				this.moveDelayRange = moveDelayRange;
				this.hesitate = hesitate;
				this.twirlAmount = twirlAmount;
				this.scale = scale;
			}
		}

		public class MarbleDrop : AbstractLevelPropertyGroup
		{
			public readonly float marbleSpeed;

			public readonly string[] marblePositionStrings;

			public readonly float marbleDelay;

			public readonly float marbleInitalDelay;

			public readonly float hesitate;

			public MarbleDrop(float marbleSpeed, string[] marblePositionStrings, float marbleDelay, float marbleInitalDelay, float hesitate)
			{
				this.marbleSpeed = marbleSpeed;
				this.marblePositionStrings = marblePositionStrings;
				this.marbleDelay = marbleDelay;
				this.marbleInitalDelay = marbleInitalDelay;
				this.hesitate = hesitate;
			}
		}

		public DicePalaceRoulette(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 650f;
				break;
			case Level.Mode.Hard:
				timeline.health = 800f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "D":
				return Pattern.Default;
			case "T":
				return Pattern.Twirl;
			case "M":
				return Pattern.Marble;
			default:
				Debug.LogError("Pattern DicePalaceRoulette.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static DicePalaceRoulette GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Platform(0f, 0f, 0f, 0f), new Twirl(0f, 0f, 0f, new string[1] { "5,6,3,6,4,5,6,3,4,7,4,3" }, 0f), new MarbleDrop(0f, new string[0], 0f, 0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 650;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Twirl,
					Pattern.Marble
				} }, States.Main, new Platform(0f, 30f, 4f, 2.2f), new Twirl(800f, 0.0001f, 1f, new string[2] { "4,5,3,6,4,5,4,3,4,6,4,3", "5,3,4,4,3,5,4,3,6,4,3,3" }, 1f), new MarbleDrop(990f, new string[12]
				{
					"25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,700,650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,400,450,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350,400,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,400-700,650,600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,400-700,450-650,500", "25-1050,100-1000,150-950,200-900,250-850,300-800,750,700,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350,400,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,700,650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,400,450,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350,400,450-650,500-600,550",
					"25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,700,650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,400-700,450,500,550"
				}, 0.27f, 1.3f, 2f)));
				break;
			case Level.Mode.Hard:
				hp = 800;
				goalTimes = new Level.GoalTimes(60f, 60f, 60f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Twirl,
					Pattern.Marble
				} }, States.Main, new Platform(0f, 30f, 4f, 1.8f), new Twirl(950f, 0.0001f, 0.8f, new string[1] { "5,6,3,6,4,5,6,3,4,7,4,3" }, 1f), new MarbleDrop(1200f, new string[16]
				{
					"25-1050,100-1000,150-950,200-900,250-850,300,350-750,400-700,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,700,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,750,400-700,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,400,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,400-700,450-650,600,550", "25-1050,100-1000,150-950,200-900,250-850,800,350-750,400-700,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,400-700,450,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,400-700,650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,400-700,450,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300,350-750,400-700,450-650,500-600,550",
					"25-1050,100-1000,150-950,200-900,250-850,300-800,750,400-700,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,750,400-700,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350,400-700,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,700,450-650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350-750,400-700,650,500-600,550", "25-1050,100-1000,150-950,200-900,250-850,300-800,350,400-700,450-650,500-600,550"
				}, 0.24f, 1f, 1.6f)));
				break;
			}
			return new DicePalaceRoulette(hp, goalTimes, list.ToArray());
		}
	}

	public class DicePalaceTest : AbstractLevelProperties<DicePalaceTest.State, DicePalaceTest.Pattern, DicePalaceTest.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected DicePalaceTest properties { get; private set; }

			public virtual void LevelInit(DicePalaceTest properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public State(float healthTrigger, Pattern[][] patterns, States stateName)
				: base(healthTrigger, patterns, stateName)
			{
			}
		}

		public DicePalaceTest(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 100f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern DicePalaceTest.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static DicePalaceTest GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			case Level.Mode.Normal:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			}
			return new DicePalaceTest(hp, goalTimes, list.ToArray());
		}
	}

	public class Dragon : AbstractLevelProperties<Dragon.State, Dragon.Pattern, Dragon.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Dragon properties { get; private set; }

			public virtual void LevelInit(Dragon properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			ThreeHeads = 2,
			FireMarchers = 3
		}

		public enum Pattern
		{
			Meteor = 0,
			Peashot = 1,
			Uninitialized = 2
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Meteor meteor;

			public readonly Tail tail;

			public readonly Peashot peashot;

			public readonly FireAndSmoke fireAndSmoke;

			public readonly FireMarchers fireMarchers;

			public readonly Potions potions;

			public readonly Blowtorch blowtorch;

			public readonly Clouds clouds;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Meteor meteor, Tail tail, Peashot peashot, FireAndSmoke fireAndSmoke, FireMarchers fireMarchers, Potions potions, Blowtorch blowtorch, Clouds clouds)
				: base(healthTrigger, patterns, stateName)
			{
				this.meteor = meteor;
				this.tail = tail;
				this.peashot = peashot;
				this.fireAndSmoke = fireAndSmoke;
				this.fireMarchers = fireMarchers;
				this.potions = potions;
				this.blowtorch = blowtorch;
				this.clouds = clouds;
			}
		}

		public class Meteor : AbstractLevelPropertyGroup
		{
			public readonly string[] pattern;

			public readonly float speedX;

			public readonly float timeY;

			public readonly float shotDelay;

			public readonly float hesitate;

			public Meteor(string[] pattern, float speedX, float timeY, float shotDelay, float hesitate)
			{
				this.pattern = pattern;
				this.speedX = speedX;
				this.timeY = timeY;
				this.shotDelay = shotDelay;
				this.hesitate = hesitate;
			}
		}

		public class Tail : AbstractLevelPropertyGroup
		{
			public readonly bool active;

			public readonly float warningTime;

			public readonly float inTime;

			public readonly float outTime;

			public readonly float holdTime;

			public readonly MinMax attackDelay;

			public Tail(bool active, float warningTime, float inTime, float outTime, float holdTime, MinMax attackDelay)
			{
				this.active = active;
				this.warningTime = warningTime;
				this.inTime = inTime;
				this.outTime = outTime;
				this.holdTime = holdTime;
				this.attackDelay = attackDelay;
			}
		}

		public class Peashot : AbstractLevelPropertyGroup
		{
			public readonly string[] patternString;

			public readonly float shotDelay;

			public readonly float speed;

			public readonly string colorString;

			public readonly float hesitate;

			public Peashot(string[] patternString, float shotDelay, float speed, string colorString, float hesitate)
			{
				this.patternString = patternString;
				this.shotDelay = shotDelay;
				this.speed = speed;
				this.colorString = colorString;
				this.hesitate = hesitate;
			}
		}

		public class FireAndSmoke : AbstractLevelPropertyGroup
		{
			public readonly string PatternString;

			public FireAndSmoke(string PatternString)
			{
				this.PatternString = PatternString;
			}
		}

		public class FireMarchers : AbstractLevelPropertyGroup
		{
			public readonly float moveSpeed;

			public readonly float spawnDelay;

			public readonly MinMax jumpDelay;

			public readonly float crouchTime;

			public readonly float gravity;

			public readonly MinMax jumpSpeed;

			public readonly MinMax jumpAngle;

			public readonly MinMax jumpX;

			public FireMarchers(float moveSpeed, float spawnDelay, MinMax jumpDelay, float crouchTime, float gravity, MinMax jumpSpeed, MinMax jumpAngle, MinMax jumpX)
			{
				this.moveSpeed = moveSpeed;
				this.spawnDelay = spawnDelay;
				this.jumpDelay = jumpDelay;
				this.crouchTime = crouchTime;
				this.gravity = gravity;
				this.jumpSpeed = jumpSpeed;
				this.jumpAngle = jumpAngle;
				this.jumpX = jumpX;
			}
		}

		public class Potions : AbstractLevelPropertyGroup
		{
			public readonly float potionSpeed;

			public readonly float spitBulletSpeed;

			public readonly float potionHP;

			public readonly string[] potionTypeString;

			public readonly string[] shotPositionString;

			public readonly float potionScale;

			public readonly float explosionBulletScale;

			public readonly string[] attackCount;

			public readonly float repeatDelay;

			public readonly float attackMainDelay;

			public readonly float playerAimCount;

			public Potions(float potionSpeed, float spitBulletSpeed, float potionHP, string[] potionTypeString, string[] shotPositionString, float potionScale, float explosionBulletScale, string[] attackCount, float repeatDelay, float attackMainDelay, float playerAimCount)
			{
				this.potionSpeed = potionSpeed;
				this.spitBulletSpeed = spitBulletSpeed;
				this.potionHP = potionHP;
				this.potionTypeString = potionTypeString;
				this.shotPositionString = shotPositionString;
				this.potionScale = potionScale;
				this.explosionBulletScale = explosionBulletScale;
				this.attackCount = attackCount;
				this.repeatDelay = repeatDelay;
				this.attackMainDelay = attackMainDelay;
				this.playerAimCount = playerAimCount;
			}
		}

		public class Blowtorch : AbstractLevelPropertyGroup
		{
			public readonly string[] attackDelayString;

			public readonly int warningDurationOne;

			public readonly int warningDurationTwo;

			public readonly float fireOnDuration;

			public readonly float repeatDelay;

			public readonly float fireSize;

			public Blowtorch(string[] attackDelayString, int warningDurationOne, int warningDurationTwo, float fireOnDuration, float repeatDelay, float fireSize)
			{
				this.attackDelayString = attackDelayString;
				this.warningDurationOne = warningDurationOne;
				this.warningDurationTwo = warningDurationTwo;
				this.fireOnDuration = fireOnDuration;
				this.repeatDelay = repeatDelay;
				this.fireSize = fireSize;
			}
		}

		public class Clouds : AbstractLevelPropertyGroup
		{
			public readonly string[] cloudPositions;

			public readonly float cloudSpeed;

			public readonly bool movingRight;

			public readonly float cloudDelay;

			public Clouds(string[] cloudPositions, float cloudSpeed, bool movingRight, float cloudDelay)
			{
				this.cloudPositions = cloudPositions;
				this.cloudSpeed = cloudSpeed;
				this.movingRight = movingRight;
				this.cloudDelay = cloudDelay;
			}
		}

		public Dragon(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1200f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.75f));
				timeline.events.Add(new Level.Timeline.Event("FireMarchers", 0.45f));
				break;
			case Level.Mode.Normal:
				timeline.health = 1700f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.92f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.82f));
				timeline.events.Add(new Level.Timeline.Event("FireMarchers", 0.63f));
				timeline.events.Add(new Level.Timeline.Event("ThreeHeads", 0.33f));
				break;
			case Level.Mode.Hard:
				timeline.health = 1900f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.9f));
				timeline.events.Add(new Level.Timeline.Event("FireMarchers", 0.64f));
				timeline.events.Add(new Level.Timeline.Event("ThreeHeads", 0.35f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "M":
				return Pattern.Meteor;
			case "P":
				return Pattern.Peashot;
			default:
				Debug.LogError("Pattern Dragon.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static Dragon GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1200;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[11]
				{
					Pattern.Peashot,
					Pattern.Meteor,
					Pattern.Peashot,
					Pattern.Peashot,
					Pattern.Meteor,
					Pattern.Peashot,
					Pattern.Meteor,
					Pattern.Peashot,
					Pattern.Peashot,
					Pattern.Peashot,
					Pattern.Meteor
				} }, States.Main, new Meteor(new string[10] { "UD", "DUD", "UD", "UDU", "DD", "DUD", "UD", "UDU", "UU", "DUD" }, 360f, 1.2f, 2.3f, 2f), new Tail(false, 1f, 0.15f, 2f, 1f, new MinMax(2.5f, 4f)), new Peashot(new string[5] { "0.5,P,1.5,P", "1,P,2,P", "0.5,P", "1,P,1,P", "1,P" }, 0.25f, 600f, "OBP", 2f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(285f, 0.75f, new MinMax(1.5f, 2.5f), 0.5f, 2000f, new MinMax(0f, 1600f), new MinMax(45f, 75f), new MinMax(150f, 650f)), new Potions(500f, 800f, 5f, new string[1] { "H" }, new string[1] { "T:A,B:C" }, 0.8f, 0.6f, new string[1] { "4,6,5" }, 1.2f, 4f, 3f), new Blowtorch(new string[1] { "8,10,14,8,10" }, 12, 9, 2f, 2f, 0.8f), new Clouds(new string[7] { "205,D0.2,620,D0.7,415,D0.4,235,580,D1,505,D0.6,345,D0.2,150,D0.5,565,D0.4,375,D0.8", "560,D0.2,265,D0.8,405,D0.4,150,570,D0.9,250,540,D0.8,370,D0.8", "445,D0.6,290,D0.6,565,D0.6,150,D1", "226,D0.3,630,D0.5,374,D1,525,245,D0.5,394,D0.7,580,D0.1,285,D0.5,410,D0.9", "500,205,D1,422,D0.9", "180,520,D0.8,600,320,D0.8,220,D0.3,490,D0.6,350,D0.9", "575,D0.3,228,D0.7,335,D1" }, 135f, false, 0.71f)));
				list.Add(new State(0.75f, new Pattern[1][] { new Pattern[10]
				{
					Pattern.Peashot,
					Pattern.Meteor,
					Pattern.Peashot,
					Pattern.Peashot,
					Pattern.Meteor,
					Pattern.Peashot,
					Pattern.Meteor,
					Pattern.Peashot,
					Pattern.Peashot,
					Pattern.Meteor
				} }, States.Generic, new Meteor(new string[10] { "UD", "DUD", "UD", "UDU", "DD", "DUD", "UD", "UDU", "UU", "DUD" }, 360f, 1.2f, 2.3f, 2f), new Tail(true, 1f, 0.5f, 1.4f, 1f, new MinMax(2.9f, 4.3f)), new Peashot(new string[5] { "0.5,P,1.5,P", "1,P,2,P", "0.5,P", "1,P,1,P", "1,P" }, 0.25f, 600f, "OBP", 2f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(285f, 0.75f, new MinMax(1.5f, 2.5f), 0.5f, 2000f, new MinMax(0f, 1600f), new MinMax(45f, 75f), new MinMax(150f, 650f)), new Potions(500f, 800f, 5f, new string[1] { "H" }, new string[1] { "T:A,B:C" }, 0.8f, 0.6f, new string[1] { "4,6,5" }, 1.2f, 4f, 3f), new Blowtorch(new string[1] { "8,10,14,8,10" }, 12, 9, 2f, 2f, 0.8f), new Clouds(new string[7] { "205,D0.2,620,D0.7,415,D0.4,235,580,D1,505,D0.6,345,D0.2,150,D0.5,565,D0.4,375,D0.8", "560,D0.2,265,D0.8,405,D0.4,150,570,D0.9,250,540,D0.8,370,D0.8", "445,D0.6,290,D0.6,565,D0.6,150,D1", "226,D0.3,630,D0.5,374,D1,525,245,D0.5,394,D0.7,580,D0.1,285,D0.5,410,D0.9", "500,205,D1,422,D0.9", "180,520,D0.8,600,320,D0.8,220,D0.3,490,D0.6,350,D0.9", "575,D0.3,228,D0.7,335,D1" }, 135f, false, 0.71f)));
				list.Add(new State(0.45f, new Pattern[1][] { new Pattern[0] }, States.FireMarchers, new Meteor(new string[10] { "UD", "DUD", "UD", "UDU", "DD", "DUD", "UD", "UDU", "UU", "DUD" }, 360f, 1.2f, 2.3f, 2f), new Tail(true, 1f, 0.5f, 1.4f, 1f, new MinMax(2.9f, 4.3f)), new Peashot(new string[5] { "0.5,P,1.5,P", "1,P,2,P", "0.5,P", "1,P,1,P", "1,P" }, 0.25f, 600f, "OBP", 2f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(285f, 0.75f, new MinMax(1.5f, 2.5f), 0.5f, 2000f, new MinMax(0f, 1600f), new MinMax(45f, 75f), new MinMax(150f, 650f)), new Potions(500f, 800f, 5f, new string[1] { "H" }, new string[1] { "T:A,B:C" }, 0.8f, 0.6f, new string[1] { "4,6,5" }, 1.2f, 4f, 3f), new Blowtorch(new string[1] { "8,10,14,8,10" }, 12, 9, 2f, 2f, 0.8f), new Clouds(new string[7] { "205,D0.2,620,D0.7,415,D0.4,235,580,D1,505,D0.6,345,D0.2,150,D0.5,565,D0.4,375,D0.8", "560,D0.2,265,D0.8,405,D0.4,150,570,D0.9,250,540,D0.8,370,D0.8", "445,D0.6,290,D0.6,565,D0.6,150,D1", "226,D0.3,630,D0.5,374,D1,525,245,D0.5,394,D0.7,580,D0.1,285,D0.5,410,D0.9", "500,205,D1,422,D0.9", "180,520,D0.8,600,320,D0.8,220,D0.3,490,D0.6,350,D0.9", "575,D0.3,228,D0.7,335,D1" }, 135f, false, 0.71f)));
				break;
			case Level.Mode.Normal:
				hp = 1700;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] { Pattern.Peashot } }, States.Main, new Meteor(new string[4] { "UDUD", "DUD", "DUDU", "UDU" }, 400f, 1f, 1.5f, 2f), new Tail(false, 1f, 0.15f, 2f, 4f, new MinMax(2.5f, 4f)), new Peashot(new string[3] { "0.5,P,0.8,P,1,P", "0.5,P,1,P", "0.5,P,0.8,P" }, 0.2f, 650f, "OBP", 2f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(350f, 0.5f, new MinMax(1.3f, 2.4f), 0.5f, 2100f, new MinMax(0f, 1650f), new MinMax(45f, 70f), new MinMax(150f, 650f)), new Potions(400f, 580f, 5f, new string[1] { "X" }, new string[4] { "T:A,T:C,B:A,B:C,T:A,T:C", "T:A,B:A,T:C,B:C,T:A,B:A", "B:C,B:A,T:C,T:A,B:C,B:A", "B:A,T:A,B:C,T:C,B:A,T:A" }, 0.7f, 0.6f, new string[1] { "6,8,8,6,10,8,6,6,8,10" }, 0f, 1.8f, 3f), new Blowtorch(new string[1] { "10,14,8,13,11,10,15,10,8,13,12,15" }, 12, 20, 1.5f, 2f, 0.8f), new Clouds(new string[7] { "500,205,D1,405,D0.8", "180,520,D0.8,600,320,D0.8,220,D0.3,490,D0.6,350,D0.8", "560,D0.2,265,D0.8,405,D0.4,190,570,D0.9,250,540,D0.8,370,D0.6", "575,D0.3,228,D0.7,335,D0.9", "205,D0.2,620,D0.7,415,D0.4,235,580,D1,505,D0.6,345,D0.2,160,D0.5,565,D0.4,375,D0.7", "226,D0.3,630,D0.5,374,D1,525,245,D0.5,394,D0.7,580,D0.1,285,D0.5,410,D0.8", "445,D0.6,290,D0.6,565,D0.6,150,D0.7,330,D0.7" }, 210f, false, 0.37f)));
				list.Add(new State(0.92f, new Pattern[1][] { new Pattern[1] }, States.Generic, new Meteor(new string[4] { "UDUD", "DUD", "DUDU", "UDU" }, 400f, 1f, 1.5f, 2f), new Tail(false, 1f, 0.15f, 2f, 4f, new MinMax(2.5f, 4f)), new Peashot(new string[3] { "0.5,P,0.8,P,1,P", "0.5,P,1,P", "0.5,P,0.8,P" }, 0.2f, 650f, "OBP", 2f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(350f, 0.5f, new MinMax(1.3f, 2.4f), 0.5f, 2100f, new MinMax(0f, 1650f), new MinMax(45f, 70f), new MinMax(150f, 650f)), new Potions(400f, 580f, 5f, new string[1] { "X" }, new string[4] { "T:A,T:C,B:A,B:C,T:A,T:C", "T:A,B:A,T:C,B:C,T:A,B:A", "B:C,B:A,T:C,T:A,B:C,B:A", "B:A,T:A,B:C,T:C,B:A,T:A" }, 0.7f, 0.6f, new string[1] { "6,8,8,6,10,8,6,6,8,10" }, 0f, 1.8f, 3f), new Blowtorch(new string[1] { "10,14,8,13,11,10,15,10,8,13,12,15" }, 12, 20, 1.5f, 2f, 0.8f), new Clouds(new string[7] { "500,205,D1,405,D0.8", "180,520,D0.8,600,320,D0.8,220,D0.3,490,D0.6,350,D0.8", "560,D0.2,265,D0.8,405,D0.4,190,570,D0.9,250,540,D0.8,370,D0.6", "575,D0.3,228,D0.7,335,D0.9", "205,D0.2,620,D0.7,415,D0.4,235,580,D1,505,D0.6,345,D0.2,160,D0.5,565,D0.4,375,D0.7", "226,D0.3,630,D0.5,374,D1,525,245,D0.5,394,D0.7,580,D0.1,285,D0.5,410,D0.8", "445,D0.6,290,D0.6,565,D0.6,150,D0.7,330,D0.7" }, 210f, false, 0.37f)));
				list.Add(new State(0.82f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Meteor,
					Pattern.Peashot
				} }, States.Generic, new Meteor(new string[4] { "UB", "BD", "DB", "BU" }, 500f, 1f, 1.5f, 2f), new Tail(true, 1.5f, 0.15f, 1f, 0.5f, new MinMax(3.5f, 5.5f)), new Peashot(new string[3] { "1,P,1,P", "1,P", "1,P" }, 0.15f, 750f, "OBOP", 1.5f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(350f, 0.5f, new MinMax(1.3f, 2.4f), 0.5f, 2100f, new MinMax(0f, 1650f), new MinMax(45f, 70f), new MinMax(150f, 650f)), new Potions(400f, 580f, 5f, new string[1] { "X" }, new string[4] { "T:A,T:C,B:A,B:C,T:A,T:C", "T:A,B:A,T:C,B:C,T:A,B:A", "B:C,B:A,T:C,T:A,B:C,B:A", "B:A,T:A,B:C,T:C,B:A,T:A" }, 0.7f, 0.6f, new string[1] { "6,8,8,6,10,8,6,6,8,10" }, 0f, 1.8f, 3f), new Blowtorch(new string[1] { "10,14,8,13,11,10,15,10,8,13,12,15" }, 12, 20, 1.5f, 2f, 0.8f), new Clouds(new string[7] { "500,205,D1,405,D0.8", "180,520,D0.8,600,320,D0.8,220,D0.3,490,D0.6,350,D0.8", "560,D0.2,265,D0.8,405,D0.4,190,570,D0.9,250,540,D0.8,370,D0.6", "575,D0.3,228,D0.7,335,D0.9", "205,D0.2,620,D0.7,415,D0.4,235,580,D1,505,D0.6,345,D0.2,160,D0.5,565,D0.4,375,D0.7", "226,D0.3,630,D0.5,374,D1,525,245,D0.5,394,D0.7,580,D0.1,285,D0.5,410,D0.8", "445,D0.6,290,D0.6,565,D0.6,150,D0.7,330,D0.7" }, 210f, false, 0.37f)));
				list.Add(new State(0.63f, new Pattern[1][] { new Pattern[1] { Pattern.Peashot } }, States.FireMarchers, new Meteor(new string[4] { "UB", "BD", "DB", "BU" }, 500f, 1f, 1.5f, 2f), new Tail(true, 1.5f, 0.15f, 1f, 0.5f, new MinMax(3.5f, 5.5f)), new Peashot(new string[3] { "1,P,1,P", "1,P", "1,P" }, 0.15f, 750f, "OBOP", 1.5f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(350f, 0.5f, new MinMax(1.3f, 2.4f), 0.5f, 2100f, new MinMax(0f, 1650f), new MinMax(45f, 70f), new MinMax(150f, 650f)), new Potions(400f, 580f, 5f, new string[1] { "X" }, new string[4] { "T:A,T:C,B:A,B:C,T:A,T:C", "T:A,B:A,T:C,B:C,T:A,B:A", "B:C,B:A,T:C,T:A,B:C,B:A", "B:A,T:A,B:C,T:C,B:A,T:A" }, 0.7f, 0.6f, new string[1] { "6,8,8,6,10,8,6,6,8,10" }, 0f, 1.8f, 3f), new Blowtorch(new string[1] { "10,14,8,13,11,10,15,10,8,13,12,15" }, 12, 20, 1.5f, 2f, 0.8f), new Clouds(new string[7] { "500,205,D1,405,D0.8", "180,520,D0.8,600,320,D0.8,220,D0.3,490,D0.6,350,D0.8", "560,D0.2,265,D0.8,405,D0.4,190,570,D0.9,250,540,D0.8,370,D0.6", "575,D0.3,228,D0.7,335,D0.9", "205,D0.2,620,D0.7,415,D0.4,235,580,D1,505,D0.6,345,D0.2,160,D0.5,565,D0.4,375,D0.7", "226,D0.3,630,D0.5,374,D1,525,245,D0.5,394,D0.7,580,D0.1,285,D0.5,410,D0.8", "445,D0.6,290,D0.6,565,D0.6,150,D0.7,330,D0.7" }, 210f, false, 0.37f)));
				list.Add(new State(0.33f, new Pattern[1][] { new Pattern[0] }, States.ThreeHeads, new Meteor(new string[4] { "UB", "BD", "DB", "BU" }, 500f, 1f, 1.5f, 2f), new Tail(true, 1.5f, 0.15f, 1f, 0.5f, new MinMax(3.5f, 5.5f)), new Peashot(new string[3] { "1,P,1,P", "1,P", "1,P" }, 0.15f, 750f, "OBOP", 1.5f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(350f, 0.5f, new MinMax(1.3f, 2.4f), 0.5f, 2100f, new MinMax(0f, 1650f), new MinMax(45f, 70f), new MinMax(150f, 650f)), new Potions(400f, 580f, 5f, new string[1] { "X" }, new string[4] { "T:A,T:C,B:A,B:C,T:A,T:C", "T:A,B:A,T:C,B:C,T:A,B:A", "B:C,B:A,T:C,T:A,B:C,B:A", "B:A,T:A,B:C,T:C,B:A,T:A" }, 0.7f, 0.6f, new string[1] { "6,8,8,6,10,8,6,6,8,10" }, 0f, 1.8f, 3f), new Blowtorch(new string[1] { "10,14,8,13,11,10,15,10,8,13,12,15" }, 12, 20, 1.5f, 2f, 0.8f), new Clouds(new string[7] { "500,205,D1,405,D0.8", "180,520,D0.8,600,320,D0.8,220,D0.3,490,D0.6,350,D0.8", "560,D0.2,265,D0.8,405,D0.4,190,570,D0.9,250,540,D0.8,370,D0.6", "575,D0.3,228,D0.7,335,D0.9", "205,D0.2,620,D0.7,415,D0.4,235,580,D1,505,D0.6,345,D0.2,160,D0.5,565,D0.4,375,D0.7", "226,D0.3,630,D0.5,374,D1,525,245,D0.5,394,D0.7,580,D0.1,285,D0.5,410,D0.8", "445,D0.6,290,D0.6,565,D0.6,150,D0.7,330,D0.7" }, 210f, false, 0.37f)));
				break;
			case Level.Mode.Hard:
				hp = 1900;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Peashot,
					Pattern.Meteor
				} }, States.Main, new Meteor(new string[5] { "UD", "DUD", "DU", "UD", "UDU" }, 350f, 0.85f, 1.1f, 2f), new Tail(false, 1f, 0.25f, 1f, 0.5f, new MinMax(2.5f, 4f)), new Peashot(new string[3] { "0.4,P,0.7,P", "0.7,P,0.4,P", "0.4,P,0.7,P,0.6,P" }, 0.18f, 865f, "OBP", 2f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(360f, 0.46f, new MinMax(0.9f, 2f), 0.5f, 2200f, new MinMax(200f, 1700f), new MinMax(45f, 70f), new MinMax(150f, 800f)), new Potions(460f, 700f, 5f, new string[1] { "X" }, new string[4] { "T:A,T:C,B:A,B:C,T:A,T:C", "B:A,T:A,T:C,B:A,T:A,T:C", "B:A,B:C,T:A,T:C,B:A,B:C", "T:C,B:C,B:A,T:C,B:C,B:A" }, 0.7f, 0.6f, new string[1] { "12,6,8,10,6,10,12,8" }, 0f, 1.5f, 3f), new Blowtorch(new string[1] { "11,14,8,13,11,8,15,10,8,12,14" }, 12, 20, 1.5f, 2f, 0.8f), new Clouds(new string[7] { "445,D0.6,290,D0.6,565,D0.3,190,D0.5,330,D0.6", "180,485,D0.8,560,310,D0.8,240,D0.3,570,D0.6,350,D0.7", "505,D0.3,218,480,D0.7,305,D0.8", "205,D0.2,580,D0.7,415,D0.4,235,560,D0.8,505,D0.6,345,D0.2,150,D0.5,565,D0.4,375,D0.7", "560,D0.2,285,D0.8,425,D0.6,170,500,D0.9,250,530,D0.7,380,D0.8", "495,235,D0.7,402,D0.6", "246,D0.3,550,D0.6,394,D0.8,525,295,D0.6,394,D0.6,515,D0.1,265,D0.5,370,D0.7" }, 225f, true, 0.36f)));
				list.Add(new State(0.9f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Meteor,
					Pattern.Peashot
				} }, States.Generic, new Meteor(new string[5] { "U,D", "D,U", "D,B", "U,D", "D,U" }, 350f, 0.9f, 1.2f, 2f), new Tail(true, 1.3f, 0.25f, 1f, 0.5f, new MinMax(3f, 4.5f)), new Peashot(new string[3] { "0.4,P,0.7,P", "0.7,P,0.4,P", "0.4,P,0.7,P,0.6,P" }, 0.18f, 865f, "OBP", 2f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(360f, 0.46f, new MinMax(0.9f, 2f), 0.5f, 2200f, new MinMax(200f, 1700f), new MinMax(45f, 70f), new MinMax(150f, 800f)), new Potions(460f, 700f, 5f, new string[1] { "X" }, new string[4] { "T:A,T:C,B:A,B:C,T:A,T:C", "B:A,T:A,T:C,B:A,T:A,T:C", "B:A,B:C,T:A,T:C,B:A,B:C", "T:C,B:C,B:A,T:C,B:C,B:A" }, 0.7f, 0.6f, new string[1] { "12,6,8,10,6,10,12,8" }, 0f, 1.5f, 3f), new Blowtorch(new string[1] { "11,14,8,13,11,8,15,10,8,12,14" }, 12, 20, 1.5f, 2f, 0.8f), new Clouds(new string[7] { "445,D0.6,290,D0.6,565,D0.3,190,D0.5,330,D0.6", "180,485,D0.8,560,310,D0.8,240,D0.3,570,D0.6,350,D0.7", "505,D0.3,218,480,D0.7,305,D0.8", "205,D0.2,580,D0.7,415,D0.4,235,560,D0.8,505,D0.6,345,D0.2,150,D0.5,565,D0.4,375,D0.7", "560,D0.2,285,D0.8,425,D0.6,170,500,D0.9,250,530,D0.7,380,D0.8", "495,235,D0.7,402,D0.6", "246,D0.3,550,D0.6,394,D0.8,525,295,D0.6,394,D0.6,515,D0.1,265,D0.5,370,D0.7" }, 225f, true, 0.36f)));
				list.Add(new State(0.64f, new Pattern[1][] { new Pattern[0] }, States.FireMarchers, new Meteor(new string[5] { "U,D", "D,U", "D,B", "U,D", "D,U" }, 350f, 0.9f, 1.2f, 2f), new Tail(true, 1.3f, 0.25f, 1f, 0.5f, new MinMax(3f, 4.5f)), new Peashot(new string[3] { "0.4,P,0.7,P", "0.7,P,0.4,P", "0.4,P,0.7,P,0.6,P" }, 0.18f, 865f, "OBP", 2f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(360f, 0.46f, new MinMax(0.9f, 2f), 0.5f, 2200f, new MinMax(200f, 1700f), new MinMax(45f, 70f), new MinMax(150f, 800f)), new Potions(460f, 700f, 5f, new string[1] { "X" }, new string[4] { "T:A,T:C,B:A,B:C,T:A,T:C", "B:A,T:A,T:C,B:A,T:A,T:C", "B:A,B:C,T:A,T:C,B:A,B:C", "T:C,B:C,B:A,T:C,B:C,B:A" }, 0.7f, 0.6f, new string[1] { "12,6,8,10,6,10,12,8" }, 0f, 1.5f, 3f), new Blowtorch(new string[1] { "11,14,8,13,11,8,15,10,8,12,14" }, 12, 20, 1.5f, 2f, 0.8f), new Clouds(new string[7] { "445,D0.6,290,D0.6,565,D0.3,190,D0.5,330,D0.6", "180,485,D0.8,560,310,D0.8,240,D0.3,570,D0.6,350,D0.7", "505,D0.3,218,480,D0.7,305,D0.8", "205,D0.2,580,D0.7,415,D0.4,235,560,D0.8,505,D0.6,345,D0.2,150,D0.5,565,D0.4,375,D0.7", "560,D0.2,285,D0.8,425,D0.6,170,500,D0.9,250,530,D0.7,380,D0.8", "495,235,D0.7,402,D0.6", "246,D0.3,550,D0.6,394,D0.8,525,295,D0.6,394,D0.6,515,D0.1,265,D0.5,370,D0.7" }, 225f, true, 0.36f)));
				list.Add(new State(0.35f, new Pattern[1][] { new Pattern[0] }, States.ThreeHeads, new Meteor(new string[5] { "U,D", "D,U", "D,B", "U,D", "D,U" }, 350f, 0.9f, 1.2f, 2f), new Tail(true, 1.3f, 0.25f, 1f, 0.5f, new MinMax(3f, 4.5f)), new Peashot(new string[3] { "0.4,P,0.7,P", "0.7,P,0.4,P", "0.4,P,0.7,P,0.6,P" }, 0.18f, 865f, "OBP", 2f), new FireAndSmoke("F,S,F,F,S,F,S,F,F,F,S,F,S,F,S,F,F,S"), new FireMarchers(360f, 0.46f, new MinMax(0.9f, 2f), 0.5f, 2200f, new MinMax(200f, 1700f), new MinMax(45f, 70f), new MinMax(150f, 800f)), new Potions(460f, 700f, 5f, new string[1] { "X" }, new string[4] { "T:A,T:C,B:A,B:C,T:A,T:C", "B:A,T:A,T:C,B:A,T:A,T:C", "B:A,B:C,T:A,T:C,B:A,B:C", "T:C,B:C,B:A,T:C,B:C,B:A" }, 0.7f, 0.6f, new string[1] { "12,6,8,10,6,10,12,8" }, 0f, 1.5f, 3f), new Blowtorch(new string[1] { "11,14,8,13,11,8,15,10,8,12,14" }, 12, 20, 1.5f, 2f, 0.8f), new Clouds(new string[7] { "180,485,D0.8,560,310,D0.8,190,D0.3,570,D0.6,350,D0.6", "585,D0.3,200,480,D0.7,305,D0.7", "445,D0.6,250,D0.6,575,D0.3,180,D0.5,330,D0.6", "195,D0.2,600,D0.7,415,D0.4,175,600,D0.8,475,D0.6,345,D0.2,150,D0.5,565,D0.4,375,D0.7", "535,215,D0.7,402,D0.6", "620,D0.2,285,D0.8,425,D0.6,170,550,D0.9,220,600,D0.7,380,D0.6", "176,D0.3,550,D0.6,394,D0.8,575,295,D0.6,394,D0.6,515,D0.1,205,D0.5,370,D0.7" }, 225f, true, 0.36f)));
				break;
			}
			return new Dragon(hp, goalTimes, list.ToArray());
		}
	}

	public class Flower : AbstractLevelProperties<Flower.State, Flower.Pattern, Flower.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Flower properties { get; private set; }

			public virtual void LevelInit(Flower properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			PhaseTwo = 2
		}

		public enum Pattern
		{
			Laser = 0,
			PodHands = 1,
			GattlingGun = 2,
			VineHands = 3,
			Nothing = 4,
			Uninitialized = 5
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Laser laser;

			public readonly PodHands podHands;

			public readonly Boomerang boomerang;

			public readonly Bullets bullets;

			public readonly PuffUp puffUp;

			public readonly GattlingGun gattlingGun;

			public readonly EnemyPlants enemyPlants;

			public readonly VineHands vineHands;

			public readonly PollenSpit pollenSpit;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Laser laser, PodHands podHands, Boomerang boomerang, Bullets bullets, PuffUp puffUp, GattlingGun gattlingGun, EnemyPlants enemyPlants, VineHands vineHands, PollenSpit pollenSpit)
				: base(healthTrigger, patterns, stateName)
			{
				this.laser = laser;
				this.podHands = podHands;
				this.boomerang = boomerang;
				this.bullets = bullets;
				this.puffUp = puffUp;
				this.gattlingGun = gattlingGun;
				this.enemyPlants = enemyPlants;
				this.vineHands = vineHands;
				this.pollenSpit = pollenSpit;
			}
		}

		public class Laser : AbstractLevelPropertyGroup
		{
			public readonly float anticHold;

			public readonly float attackHold;

			public readonly string attackType;

			public readonly float hesitateAfterAttack;

			public Laser(float anticHold, float attackHold, string attackType, float hesitateAfterAttack)
			{
				this.anticHold = anticHold;
				this.attackHold = attackHold;
				this.attackType = attackType;
				this.hesitateAfterAttack = hesitateAfterAttack;
			}
		}

		public class PodHands : AbstractLevelPropertyGroup
		{
			public readonly float attackDelay;

			public readonly float attackHold;

			public readonly float hesitateAfterAttack;

			public readonly string attackAmount;

			public readonly string attacktype;

			public PodHands(float attackDelay, float attackHold, float hesitateAfterAttack, string attackAmount, string attacktype)
			{
				this.attackDelay = attackDelay;
				this.attackHold = attackHold;
				this.hesitateAfterAttack = hesitateAfterAttack;
				this.attackAmount = attackAmount;
				this.attacktype = attacktype;
			}
		}

		public class Boomerang : AbstractLevelPropertyGroup
		{
			public readonly int speed;

			public readonly float offScreenDelay;

			public readonly float holdDelay;

			public readonly float initialMovementDelay;

			public Boomerang(int speed, float offScreenDelay, float holdDelay, float initialMovementDelay)
			{
				this.speed = speed;
				this.offScreenDelay = offScreenDelay;
				this.holdDelay = holdDelay;
				this.initialMovementDelay = initialMovementDelay;
			}
		}

		public class Bullets : AbstractLevelPropertyGroup
		{
			public readonly float delayNextShot;

			public readonly MinMax speedMinMax;

			public readonly float acceleration;

			public readonly int holdDelay;

			public readonly int numberOfProjectiles;

			public Bullets(float delayNextShot, MinMax speedMinMax, float acceleration, int holdDelay, int numberOfProjectiles)
			{
				this.delayNextShot = delayNextShot;
				this.speedMinMax = speedMinMax;
				this.acceleration = acceleration;
				this.holdDelay = holdDelay;
				this.numberOfProjectiles = numberOfProjectiles;
			}
		}

		public class PuffUp : AbstractLevelPropertyGroup
		{
			public readonly int speed;

			public readonly float delayExplosion;

			public readonly int holdDelay;

			public PuffUp(int speed, float delayExplosion, int holdDelay)
			{
				this.speed = speed;
				this.delayExplosion = delayExplosion;
				this.holdDelay = holdDelay;
			}
		}

		public class GattlingGun : AbstractLevelPropertyGroup
		{
			public readonly float loopDuration;

			public readonly float initialSeedDelay;

			public readonly float hesitateAfterAttack;

			public readonly float fallingSeedDelay;

			public readonly string[] seedSpawnString;

			public GattlingGun(float loopDuration, float initialSeedDelay, float hesitateAfterAttack, float fallingSeedDelay, string[] seedSpawnString)
			{
				this.loopDuration = loopDuration;
				this.initialSeedDelay = initialSeedDelay;
				this.hesitateAfterAttack = hesitateAfterAttack;
				this.fallingSeedDelay = fallingSeedDelay;
				this.seedSpawnString = seedSpawnString;
			}
		}

		public class EnemyPlants : AbstractLevelPropertyGroup
		{
			public readonly int chomperPlantHP;

			public readonly int venusPlantHP;

			public readonly int fallingSeedSpeed;

			public readonly int venusTurningSpeed;

			public readonly float venusTurningDelay;

			public readonly int venusMovmentSpeed;

			public readonly int miniFlowerPlantHP;

			public readonly int miniFlowerMovmentSpeed;

			public readonly MinMax miniFlowerShootDelay;

			public readonly int miniFlowerProjectileSpeed;

			public readonly int miniFlowerFriendHP;

			public readonly int miniFlowerProjectileDamage;

			public EnemyPlants(int chomperPlantHP, int venusPlantHP, int fallingSeedSpeed, int venusTurningSpeed, float venusTurningDelay, int venusMovmentSpeed, int miniFlowerPlantHP, int miniFlowerMovmentSpeed, MinMax miniFlowerShootDelay, int miniFlowerProjectileSpeed, int miniFlowerFriendHP, int miniFlowerProjectileDamage)
			{
				this.chomperPlantHP = chomperPlantHP;
				this.venusPlantHP = venusPlantHP;
				this.fallingSeedSpeed = fallingSeedSpeed;
				this.venusTurningSpeed = venusTurningSpeed;
				this.venusTurningDelay = venusTurningDelay;
				this.venusMovmentSpeed = venusMovmentSpeed;
				this.miniFlowerPlantHP = miniFlowerPlantHP;
				this.miniFlowerMovmentSpeed = miniFlowerMovmentSpeed;
				this.miniFlowerShootDelay = miniFlowerShootDelay;
				this.miniFlowerProjectileSpeed = miniFlowerProjectileSpeed;
				this.miniFlowerFriendHP = miniFlowerFriendHP;
				this.miniFlowerProjectileDamage = miniFlowerProjectileDamage;
			}
		}

		public class VineHands : AbstractLevelPropertyGroup
		{
			public readonly float firstPositionHold;

			public readonly float secondPositionHold;

			public readonly MinMax attackDelay;

			public readonly string handAttackString;

			public VineHands(float firstPositionHold, float secondPositionHold, MinMax attackDelay, string handAttackString)
			{
				this.firstPositionHold = firstPositionHold;
				this.secondPositionHold = secondPositionHold;
				this.attackDelay = attackDelay;
				this.handAttackString = handAttackString;
			}
		}

		public class PollenSpit : AbstractLevelPropertyGroup
		{
			public readonly string pollenAttackCount;

			public readonly float consecutiveAttackHold;

			public readonly string pollenType;

			public readonly float pollenCommaDelay;

			public readonly int pollenSpeed;

			public readonly float pollenUpDownStrength;

			public PollenSpit(string pollenAttackCount, float consecutiveAttackHold, string pollenType, float pollenCommaDelay, int pollenSpeed, float pollenUpDownStrength)
			{
				this.pollenAttackCount = pollenAttackCount;
				this.consecutiveAttackHold = consecutiveAttackHold;
				this.pollenType = pollenType;
				this.pollenCommaDelay = pollenCommaDelay;
				this.pollenSpeed = pollenSpeed;
				this.pollenUpDownStrength = pollenUpDownStrength;
			}
		}

		public Flower(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1000f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.9f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.65f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.4f));
				break;
			case Level.Mode.Normal:
				timeline.health = 1300f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.89f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.68f));
				timeline.events.Add(new Level.Timeline.Event("PhaseTwo", 0.46f));
				break;
			case Level.Mode.Hard:
				timeline.health = 1500f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.73f));
				timeline.events.Add(new Level.Timeline.Event("PhaseTwo", 0.46f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "L":
				return Pattern.Laser;
			case "H":
				return Pattern.PodHands;
			case "S":
				return Pattern.GattlingGun;
			case "V":
				return Pattern.VineHands;
			case "D":
				return Pattern.Nothing;
			default:
				Debug.LogError("Pattern Flower.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static Flower GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Laser(1.3f, 1.2f, "T,B,T,B,T,T,B,T,T,B,T,B,B,T,B", 0.5f), new PodHands(1f, 1f, 2f, "1,2,1,2,1,2,2", "B,S,S,B,S,B,B,S,B,S,S,B,S,S,B"), new Boomerang(660, 0.7f, 1.5f, 0.4f), new Bullets(0.8f, new MinMax(200f, 600f), 2f, 0, 3), new PuffUp(400, 1f, 5), new GattlingGun(1.8f, 0.5f, 0.5f, 0.9f, new string[5] { "B100,B300,B650", "B250,B550,B0", "B700,B150,B500", "B350,B50,B450", "B600,B400,B200" }), new EnemyPlants(5, 4, 320, 4, 0.9f, 110, 3, 4, new MinMax(5f, 7f), 300, 1, 25), new VineHands(0f, 0f, new MinMax(0f, 1f), string.Empty), new PollenSpit(string.Empty, 0f, string.Empty, 0f, 0, 0f)));
				list.Add(new State(0.9f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.GattlingGun,
					Pattern.Laser
				} }, States.Generic, new Laser(1.3f, 1.2f, "T,B,T,B,T,T,B,T,T,B,T,B,B,T,B", 0.5f), new PodHands(1f, 1f, 2f, "1,2,1,2,1,2,2", "B,S,S,B,S,B,B,S,B,S,S,B,S,S,B"), new Boomerang(660, 0.7f, 1.5f, 0.4f), new Bullets(0.8f, new MinMax(200f, 600f), 2f, 0, 3), new PuffUp(400, 1f, 5), new GattlingGun(1.8f, 0.5f, 0.5f, 0.9f, new string[5] { "B100,B300,B650", "B250,B550,B0", "B700,B150,B500", "B350,B50,B450", "B600,B400,B200" }), new EnemyPlants(5, 4, 320, 4, 0.9f, 110, 3, 4, new MinMax(5f, 7f), 300, 1, 25), new VineHands(0f, 0f, new MinMax(0f, 1f), string.Empty), new PollenSpit(string.Empty, 0f, string.Empty, 0f, 0, 0f)));
				list.Add(new State(0.65f, new Pattern[1][] { new Pattern[1] { Pattern.PodHands } }, States.Generic, new Laser(1.3f, 1.2f, "T,B,T,B,T,T,B,T,T,B,T,B,B,T,B", 0.5f), new PodHands(1f, 1f, 2f, "1,2,1,2,1,2,2", "B,S,S,B,S,B,B,S,B,S,S,B,S,S,B"), new Boomerang(660, 0.7f, 1.5f, 0.4f), new Bullets(0.8f, new MinMax(200f, 600f), 2f, 0, 3), new PuffUp(400, 1f, 5), new GattlingGun(1.8f, 0.5f, 0.5f, 0.9f, new string[5] { "B100,B300,B650", "B250,B550,B0", "B700,B150,B500", "B350,B50,B450", "B600,B400,B200" }), new EnemyPlants(5, 4, 320, 4, 0.9f, 110, 3, 4, new MinMax(5f, 7f), 300, 1, 25), new VineHands(0f, 0f, new MinMax(0f, 1f), string.Empty), new PollenSpit(string.Empty, 0f, string.Empty, 0f, 0, 0f)));
				list.Add(new State(0.4f, new Pattern[1][] { new Pattern[1] { Pattern.GattlingGun } }, States.Generic, new Laser(1.3f, 1.2f, "T,B,T,B,T,T,B,T,T,B,T,B,B,T,B", 0.5f), new PodHands(1f, 1f, 2f, "1,2,1,2,1,2,2", "B,S,S,B,S,B,B,S,B,S,S,B,S,S,B"), new Boomerang(660, 0.7f, 1.5f, 0.4f), new Bullets(0.8f, new MinMax(200f, 600f), 2f, 0, 3), new PuffUp(400, 1f, 5), new GattlingGun(3.5f, 0.5f, 2.5f, 0.9f, new string[6] { "A0,A100,C700,A600,A400", "A300,C500,A400,A0,A700", "A600,A500,A700,A400,C550", "C0,A700,A600,A150,A250", "A300,A550,A0,A700,C200", "A0,C650,A550,A100,A250" }), new EnemyPlants(5, 4, 320, 4, 0.9f, 110, 3, 4, new MinMax(5f, 7f), 300, 1, 25), new VineHands(0f, 0f, new MinMax(0f, 1f), string.Empty), new PollenSpit(string.Empty, 0f, string.Empty, 0f, 0, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 1300;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Laser,
					Pattern.GattlingGun
				} }, States.Main, new Laser(1f, 1f, "T,B,B,T,B,B,T,B,B", 2f), new PodHands(1f, 1f, 3f, "3,2,3,2,2,2,3,3,2,3,2,2,2,2,3,3,3,2", "B,S,S,B,S,B,S,S,S,B,S,B,S"), new Boomerang(720, 0.7f, 1.5f, 0.4f), new Bullets(0.4f, new MinMax(100f, 680f), 2f, 0, 3), new PuffUp(400, 1f, 5), new GattlingGun(4.5f, 0.5f, 3f, 0.9f, new string[10] { "A0,B300,A500,C400,A700", "B50,C650,A450,A150,A550", "A700,A300,C600,B400,A0", "B700,A0,A600,C350,B500", "A550,A150,C650,B100,A0", "A0,B250,A400,A650,C100", "B350,B650,A0,C200,A700", "A100,A200,C700,B200,A350", "A500,A200,A700,B600,C0", "A550,B150,A0,C300,A700" }), new EnemyPlants(5, 4, 320, 4, 0.9f, 120, 4, 4, new MinMax(5f, 7f), 400, 1, 25), new VineHands(0.75f, 0.2f, new MinMax(3.9f, 6.3f), "1,2,3,2,1,3"), new PollenSpit("1,2,D0.5,1,1", 2.1f, "R,R,P,R,P", 3.3f, 360, 40f)));
				list.Add(new State(0.89f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Laser,
					Pattern.PodHands
				} }, States.Generic, new Laser(1f, 1f, "T,B,B,T,B,B,T,B,B", 2f), new PodHands(1f, 1f, 3f, "3,2,3,2,2,2,3,3,2,3,2,2,2,2,3,3,3,2", "B,S,S,B,S,B,S,S,S,B,S,B,S"), new Boomerang(720, 0.7f, 1.5f, 0.4f), new Bullets(0.4f, new MinMax(100f, 680f), 2f, 0, 3), new PuffUp(400, 1f, 5), new GattlingGun(4.5f, 0.5f, 3f, 0.9f, new string[10] { "A0,B300,A500,C400,A700", "B50,C650,A450,A150,A550", "A700,A300,C600,B400,A0", "B700,A0,A600,C350,B500", "A550,A150,C650,B100,A0", "A0,B250,A400,A650,C100", "B350,B650,A0,C200,A700", "A100,A200,C700,B200,A350", "A500,A200,A700,B600,C0", "A550,B150,A0,C300,A700" }), new EnemyPlants(5, 4, 320, 4, 0.9f, 120, 4, 4, new MinMax(5f, 7f), 400, 1, 25), new VineHands(0.75f, 0.2f, new MinMax(3.9f, 6.3f), "1,2,3,2,1,3"), new PollenSpit("1,2,D0.5,1,1", 2.1f, "R,R,P,R,P", 3.3f, 360, 40f)));
				list.Add(new State(0.68f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.PodHands,
					Pattern.GattlingGun
				} }, States.Generic, new Laser(1f, 1f, "T,B,B,T,B,B,T,B,B", 2f), new PodHands(1f, 1f, 3f, "3,2,3,2,2,2,3,2,3,2,2,2,3,3,2", "B,S,S,B,B,S,B,S,S,B,B,S,B,S"), new Boomerang(720, 0.7f, 1.5f, 0.4f), new Bullets(0.4f, new MinMax(100f, 680f), 2f, 0, 3), new PuffUp(400, 1f, 5), new GattlingGun(4.5f, 0.5f, 3f, 0.9f, new string[10] { "A0,B300,A500,C400,A700", "B50,C650,A450,A150,A550", "A700,A300,C600,B400,A0", "B700,A0,A600,C350,B500", "A550,A150,C650,B100,A0", "A0,B250,A400,A650,C100", "B350,B650,A0,C200,A700", "A100,A200,C700,B200,A350", "A500,A200,A700,B600,C0", "A550,B150,A0,C300,A700" }), new EnemyPlants(5, 4, 320, 4, 0.9f, 120, 4, 4, new MinMax(5f, 7f), 400, 1, 25), new VineHands(0.75f, 0.2f, new MinMax(3.9f, 6.3f), "1,2,3,2,1,3"), new PollenSpit("1,2,D0.5,1,1", 2.1f, "R,R,P,R,P", 3.3f, 360, 40f)));
				list.Add(new State(0.46f, new Pattern[1][] { new Pattern[1] { Pattern.VineHands } }, States.PhaseTwo, new Laser(1f, 1f, "T,B,B,T,B,B,T,B,B", 2f), new PodHands(1f, 1f, 3f, "3,2,3,2,2,2,3,2,3,2,2,2,3,3,2", "B,S,S,B,B,S,B,S,S,B,B,S,B,S"), new Boomerang(720, 0.7f, 1.5f, 0.4f), new Bullets(0.4f, new MinMax(100f, 680f), 2f, 0, 3), new PuffUp(400, 1f, 5), new GattlingGun(4.5f, 0.5f, 3f, 0.9f, new string[10] { "A0,B300,A500,C400,A700", "B50,C650,A450,A150,A550", "A700,A300,C600,B400,A0", "B700,A0,A600,C350,B500", "A550,A150,C650,B100,A0", "A0,B250,A400,A650,C100", "B350,B650,A0,C200,A700", "A100,A200,C700,B200,A350", "A500,A200,A700,B600,C0", "A550,B150,A0,C300,A700" }), new EnemyPlants(5, 4, 320, 4, 0.9f, 120, 4, 4, new MinMax(5f, 7f), 400, 1, 25), new VineHands(0.75f, 0.2f, new MinMax(3.9f, 6.3f), "1,2,3,2,1,3"), new PollenSpit("1,2,D0.5,1,1", 2.1f, "R,R,P,R,P", 3.3f, 360, 40f)));
				break;
			case Level.Mode.Hard:
				hp = 1500;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] { Pattern.GattlingGun } }, States.Main, new Laser(1f, 1.5f, "T,B,B,B,T,B,B,T,B", 1f), new PodHands(1f, 1f, 2.5f, "3,2,3,2,2,2,3,3,2,3,2,2,2,2,3,3,3,2", "B,S,S,B,B,S,B,S,S,S,B,B,S,B,S"), new Boomerang(820, 0.6f, 1.5f, 0.4f), new Bullets(0.3f, new MinMax(100f, 700f), 1.8f, 0, 4), new PuffUp(400, 1f, 5), new GattlingGun(3f, 0.5f, 1f, 0.7f, new string[8] { "A50,A350,C600,A100,A700", "A250,A150,A500,C650", "A700,A450,A550,A200,C50", "A100,A250,A150,C600", "A650,A500,A700,C150,A50", "C300,A450,A250,A400,A550", "A700,A500,C300,A100", "A450,A650,C150,A550,A350" }), new EnemyPlants(8, 10, 350, 2, 0.9f, 140, 4, 4, new MinMax(4f, 7f), 400, 1, 25), new VineHands(0.6f, 0.2f, new MinMax(3.3f, 5.7f), "1,2,3,2,1,3"), new PollenSpit("1,2,D0.5,1,1", 1.5f, "R,R,P,R,R,R,P", 2.3f, 330, 50f)));
				list.Add(new State(0.73f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.PodHands,
					Pattern.Laser
				} }, States.Generic, new Laser(1f, 1.5f, "T,B,B,B,T,B,B,T,B", 1f), new PodHands(1f, 1f, 2.5f, "3,2,3,2,2,2,3,3,2,3,2,2,2,2,3,3,3,2", "B,S,S,B,B,S,B,S,S,S,B,B,S,B,S"), new Boomerang(820, 0.6f, 1.5f, 0.4f), new Bullets(0.3f, new MinMax(100f, 700f), 1.8f, 0, 4), new PuffUp(400, 1f, 5), new GattlingGun(3f, 0.5f, 1f, 0.7f, new string[8] { "A50,A350,C600,A100,A700", "A250,A150,A500,C650", "A700,A450,A550,A200,C50", "A100,A250,A150,C600", "A650,A500,A700,C150,A50", "C300,A450,A250,A400,A550", "A700,A500,C300,A100", "A450,A650,C150,A550,A350" }), new EnemyPlants(8, 10, 350, 2, 0.9f, 140, 4, 4, new MinMax(4f, 7f), 400, 1, 25), new VineHands(0.6f, 0.2f, new MinMax(3.3f, 5.7f), "1,2,3,2,1,3"), new PollenSpit("1,2,D0.5,1,1", 1.5f, "R,R,P,R,R,R,P", 2.3f, 330, 50f)));
				list.Add(new State(0.46f, new Pattern[1][] { new Pattern[0] }, States.PhaseTwo, new Laser(1f, 1.5f, "T,B,B,B,T,B,B,T,B", 1f), new PodHands(1f, 1f, 2.5f, "3,2,3,2,2,2,3,3,2,3,2,2,2,2,3,3,3,2", "B,S,S,B,B,S,B,S,S,S,B,B,S,B,S"), new Boomerang(820, 0.6f, 1.5f, 0.4f), new Bullets(0.3f, new MinMax(100f, 700f), 1.8f, 0, 4), new PuffUp(400, 1f, 5), new GattlingGun(3f, 0.5f, 1f, 0.7f, new string[8] { "A50,A350,C600,A100,A700", "A250,A150,A500,C650", "A700,A450,A550,A200,C50", "A100,A250,A150,C600", "A650,A500,A700,C150,A50", "C300,A450,A250,A400,A550", "A700,A500,C300,A100", "A450,A650,C150,A550,A350" }), new EnemyPlants(8, 10, 350, 2, 0.9f, 140, 4, 4, new MinMax(4f, 7f), 400, 1, 25), new VineHands(0.6f, 0.2f, new MinMax(3.3f, 5.7f), "1,2,3,2,1,3"), new PollenSpit("1,2,D0.5,1,1", 1.5f, "R,R,P,R,R,R,P", 2.3f, 330, 50f)));
				break;
			}
			return new Flower(hp, goalTimes, list.ToArray());
		}
	}

	public class FlyingBird : AbstractLevelProperties<FlyingBird.State, FlyingBird.Pattern, FlyingBird.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected FlyingBird properties { get; private set; }

			public virtual void LevelInit(FlyingBird properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			HouseDeath = 2,
			Whistle = 3,
			BirdRevival = 4
		}

		public enum Pattern
		{
			Feathers = 0,
			Eggs = 1,
			Lasers = 2,
			SmallBird = 3,
			Garbage = 4,
			Heart = 5,
			Default = 6,
			Uninitialized = 7
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Floating floating;

			public readonly Feathers feathers;

			public readonly Eggs eggs;

			public readonly Enemies enemies;

			public readonly Lasers lasers;

			public readonly Turrets turrets;

			public readonly SmallBird smallBird;

			public readonly BigBird bigBird;

			public readonly Nurses nurses;

			public readonly Garbage garbage;

			public readonly Heart heart;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Floating floating, Feathers feathers, Eggs eggs, Enemies enemies, Lasers lasers, Turrets turrets, SmallBird smallBird, BigBird bigBird, Nurses nurses, Garbage garbage, Heart heart)
				: base(healthTrigger, patterns, stateName)
			{
				this.floating = floating;
				this.feathers = feathers;
				this.eggs = eggs;
				this.enemies = enemies;
				this.lasers = lasers;
				this.turrets = turrets;
				this.smallBird = smallBird;
				this.bigBird = bigBird;
				this.nurses = nurses;
				this.garbage = garbage;
				this.heart = heart;
			}
		}

		public class Floating : AbstractLevelPropertyGroup
		{
			public readonly float time;

			public readonly float top;

			public readonly float bottom;

			public readonly MinMax attackInitialDelayRange;

			public Floating(float time, float top, float bottom, MinMax attackInitialDelayRange)
			{
				this.time = time;
				this.top = top;
				this.bottom = bottom;
				this.attackInitialDelayRange = attackInitialDelayRange;
			}
		}

		public class Feathers : AbstractLevelPropertyGroup
		{
			public readonly string[] pattern;

			public readonly int count;

			public readonly float speed;

			public readonly float offset;

			public readonly float shotDelay;

			public readonly float initalShotDelay;

			public readonly float hesitate;

			public Feathers(string[] pattern, int count, float speed, float offset, float shotDelay, float initalShotDelay, float hesitate)
			{
				this.pattern = pattern;
				this.count = count;
				this.speed = speed;
				this.offset = offset;
				this.shotDelay = shotDelay;
				this.initalShotDelay = initalShotDelay;
				this.hesitate = hesitate;
			}
		}

		public class Eggs : AbstractLevelPropertyGroup
		{
			public readonly string[] pattern;

			public readonly float speed;

			public readonly float shotDelay;

			public readonly float hesitate;

			public Eggs(string[] pattern, float speed, float shotDelay, float hesitate)
			{
				this.pattern = pattern;
				this.speed = speed;
				this.shotDelay = shotDelay;
				this.hesitate = hesitate;
			}
		}

		public class Enemies : AbstractLevelPropertyGroup
		{
			public readonly bool active;

			public readonly int count;

			public readonly float delay;

			public readonly int health;

			public readonly float speed;

			public readonly float floatRange;

			public readonly float floatTime;

			public readonly float projectileHeight;

			public readonly float projectileFallTime;

			public readonly float projectileDelay;

			public readonly float groupDelay;

			public readonly float initalGroupDelay;

			public readonly bool aim;

			public Enemies(bool active, int count, float delay, int health, float speed, float floatRange, float floatTime, float projectileHeight, float projectileFallTime, float projectileDelay, float groupDelay, float initalGroupDelay, bool aim)
			{
				this.active = active;
				this.count = count;
				this.delay = delay;
				this.health = health;
				this.speed = speed;
				this.floatRange = floatRange;
				this.floatTime = floatTime;
				this.projectileHeight = projectileHeight;
				this.projectileFallTime = projectileFallTime;
				this.projectileDelay = projectileDelay;
				this.groupDelay = groupDelay;
				this.initalGroupDelay = initalGroupDelay;
				this.aim = aim;
			}
		}

		public class Lasers : AbstractLevelPropertyGroup
		{
			public readonly float speed;

			public readonly float hesitate;

			public Lasers(float speed, float hesitate)
			{
				this.speed = speed;
				this.hesitate = hesitate;
			}
		}

		public class Turrets : AbstractLevelPropertyGroup
		{
			public readonly bool active;

			public readonly int health;

			public readonly float inTime;

			public readonly float bulletSpeed;

			public readonly float bulletDelay;

			public readonly float respawnDelay;

			public readonly float floatRange;

			public readonly float floatTime;

			public Turrets(bool active, int health, float inTime, float bulletSpeed, float bulletDelay, float respawnDelay, float floatRange, float floatTime)
			{
				this.active = active;
				this.health = health;
				this.inTime = inTime;
				this.bulletSpeed = bulletSpeed;
				this.bulletDelay = bulletDelay;
				this.respawnDelay = respawnDelay;
				this.floatRange = floatRange;
				this.floatTime = floatTime;
			}
		}

		public class SmallBird : AbstractLevelPropertyGroup
		{
			public readonly float timeX;

			public readonly float timeY;

			public readonly float minX;

			public readonly int eggCount;

			public readonly MinMax eggRange;

			public readonly float eggRotationSpeed;

			public readonly float eggMoveTime;

			public readonly float shotDelay;

			public readonly float shotSpeed;

			public readonly float leaveTime;

			public SmallBird(float timeX, float timeY, float minX, int eggCount, MinMax eggRange, float eggRotationSpeed, float eggMoveTime, float shotDelay, float shotSpeed, float leaveTime)
			{
				this.timeX = timeX;
				this.timeY = timeY;
				this.minX = minX;
				this.eggCount = eggCount;
				this.eggRange = eggRange;
				this.eggRotationSpeed = eggRotationSpeed;
				this.eggMoveTime = eggMoveTime;
				this.shotDelay = shotDelay;
				this.shotSpeed = shotSpeed;
				this.leaveTime = leaveTime;
			}
		}

		public class BigBird : AbstractLevelPropertyGroup
		{
			public readonly float speedXTime;

			public BigBird(float speedXTime)
			{
				this.speedXTime = speedXTime;
			}
		}

		public class Nurses : AbstractLevelPropertyGroup
		{
			public readonly float bulletSpeed;

			public readonly float pillSpeed;

			public readonly float pillMaxHeight;

			public readonly float pillExplodeDelay;

			public readonly string pinkString;

			public readonly float attackRepeatDelay;

			public readonly string attackCount;

			public readonly float attackMainDelay;

			public Nurses(float bulletSpeed, float pillSpeed, float pillMaxHeight, float pillExplodeDelay, string pinkString, float attackRepeatDelay, string attackCount, float attackMainDelay)
			{
				this.bulletSpeed = bulletSpeed;
				this.pillSpeed = pillSpeed;
				this.pillMaxHeight = pillMaxHeight;
				this.pillExplodeDelay = pillExplodeDelay;
				this.pinkString = pinkString;
				this.attackRepeatDelay = attackRepeatDelay;
				this.attackCount = attackCount;
				this.attackMainDelay = attackMainDelay;
			}
		}

		public class Garbage : AbstractLevelPropertyGroup
		{
			public readonly float maxHeight;

			public readonly float speedY;

			public readonly float speedX;

			public readonly float speedXIncreaser;

			public readonly string shotCount;

			public readonly float shotDelay;

			public readonly float shotSize;

			public readonly MinMax hesitate;

			public readonly string[] garbageTypeString;

			public Garbage(float maxHeight, float speedY, float speedX, float speedXIncreaser, string shotCount, float shotDelay, float shotSize, MinMax hesitate, string[] garbageTypeString)
			{
				this.maxHeight = maxHeight;
				this.speedY = speedY;
				this.speedX = speedX;
				this.speedXIncreaser = speedXIncreaser;
				this.shotCount = shotCount;
				this.shotDelay = shotDelay;
				this.shotSize = shotSize;
				this.hesitate = hesitate;
				this.garbageTypeString = garbageTypeString;
			}
		}

		public class Heart : AbstractLevelPropertyGroup
		{
			public readonly float movementSpeed;

			public readonly int shotCount;

			public readonly MinMax hesitate;

			public readonly MinMax spreadAngle;

			public readonly float projectileSpeed;

			public readonly float heartHeight;

			public readonly string[] shootString;

			public readonly string[] numOfProjectiles;

			public Heart(float movementSpeed, int shotCount, MinMax hesitate, MinMax spreadAngle, float projectileSpeed, float heartHeight, string[] shootString, string[] numOfProjectiles)
			{
				this.movementSpeed = movementSpeed;
				this.shotCount = shotCount;
				this.hesitate = hesitate;
				this.spreadAngle = spreadAngle;
				this.projectileSpeed = projectileSpeed;
				this.heartHeight = heartHeight;
				this.shootString = shootString;
				this.numOfProjectiles = numOfProjectiles;
			}
		}

		[CompilerGenerated]
		private static Dictionary<string, int> _003C_003Ef__switch_0024map8;

		public FlyingBird(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 2000f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.85f));
				timeline.events.Add(new Level.Timeline.Event("Whistle", 0.5f));
				timeline.events.Add(new Level.Timeline.Event("HouseDeath", 0.25f));
				break;
			case Level.Mode.Normal:
				timeline.health = 2400f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.9f));
				timeline.events.Add(new Level.Timeline.Event("Whistle", 0.75f));
				timeline.events.Add(new Level.Timeline.Event("HouseDeath", 0.52f));
				timeline.events.Add(new Level.Timeline.Event("BirdRevival", 0.29f));
				break;
			case Level.Mode.Hard:
				timeline.health = 2800f;
				timeline.events.Add(new Level.Timeline.Event("Whistle", 0.75f));
				timeline.events.Add(new Level.Timeline.Event("HouseDeath", 0.5f));
				timeline.events.Add(new Level.Timeline.Event("BirdRevival", 0.31f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null)
			{
				if (_003C_003Ef__switch_0024map8 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(7);
					dictionary.Add("F", 0);
					dictionary.Add("E", 1);
					dictionary.Add("L", 2);
					dictionary.Add("S", 3);
					dictionary.Add("G", 4);
					dictionary.Add("H", 5);
					dictionary.Add("D", 6);
					_003C_003Ef__switch_0024map8 = dictionary;
				}
				int value;
				if (_003C_003Ef__switch_0024map8.TryGetValue(id, out value))
				{
					switch (value)
					{
					case 0:
						return Pattern.Feathers;
					case 1:
						return Pattern.Eggs;
					case 2:
						return Pattern.Lasers;
					case 3:
						return Pattern.SmallBird;
					case 4:
						return Pattern.Garbage;
					case 5:
						return Pattern.Heart;
					case 6:
						return Pattern.Default;
					}
				}
			}
			Debug.LogError("Pattern FlyingBird.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static FlyingBird GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 2000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[5]
				{
					Pattern.Eggs,
					Pattern.Lasers,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Lasers
				} }, States.Main, new Floating(2.8f, 360f, -120f, new MinMax(0.5f, 2.2f)), new Feathers(new string[1] { "P:10" }, 8, 680f, 29f, 0.33f, 0.33f, 1.7f), new Eggs(new string[5] { "P:1,D:1,P:2", "D:0.5,P:2", "P:1,D:0.1,P:1", "P:2,D:1,P:1", "D:0.7,P:2" }, 470f, 1.35f, 1.4f), new Enemies(false, 4, 0.8f, 8, 300f, 100f, 2f, 600f, 1f, 10f, 5f, 0f, false), new Lasers(750f, 1.5f), new Turrets(false, 0, 0f, 0f, 0f, 0f, 0f, 0f), new SmallBird(3.45f, 2.07f, -300f, 4, new MinMax(100f, 470f), 70f, 8f, 8f, 700f, 2f), new BigBird(0f), new Nurses(0f, 0f, 0f, 0f, string.Empty, 0f, string.Empty, 0f), new Garbage(0f, 0f, 0f, 0f, string.Empty, 0f, 0f, new MinMax(0f, 1f), new string[0]), new Heart(0f, 0, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, 0f, new string[0], new string[0])));
				list.Add(new State(0.85f, new Pattern[1][] { new Pattern[9]
				{
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Lasers,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Lasers
				} }, States.Generic, new Floating(2.8f, 360f, -120f, new MinMax(0.5f, 2.2f)), new Feathers(new string[1] { "P:10" }, 8, 680f, 29f, 0.33f, 0.33f, 1.7f), new Eggs(new string[5] { "P:1,D:1,P:2", "D:0.5,P:2", "P:1,D:0.1,P:1", "P:2,D:1,P:1", "D:0.7,P:2" }, 470f, 1.35f, 1.4f), new Enemies(true, 3, 0.8f, 8, 400f, 100f, 2f, 600f, 1f, 10f, 3.3f, 1.5f, true), new Lasers(750f, 1.5f), new Turrets(false, 0, 0f, 0f, 0f, 0f, 0f, 0f), new SmallBird(3.45f, 2.07f, -300f, 4, new MinMax(100f, 470f), 70f, 8f, 8f, 700f, 2f), new BigBird(0f), new Nurses(0f, 0f, 0f, 0f, string.Empty, 0f, string.Empty, 0f), new Garbage(0f, 0f, 0f, 0f, string.Empty, 0f, 0f, new MinMax(0f, 1f), new string[0]), new Heart(0f, 0, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, 0f, new string[0], new string[0])));
				list.Add(new State(0.5f, new Pattern[1][] { new Pattern[1] }, States.Whistle, new Floating(2.8f, 250f, -120f, new MinMax(0.2f, 2.2f)), new Feathers(new string[1] { "P:10" }, 8, 680f, 29f, 0.33f, 0.33f, 1.7f), new Eggs(new string[5] { "P:1,D:1,P:2", "D:0.5,P:2", "P:1,D:0.1,P:1", "P:2,D:1,P:1", "D:0.7,P:2" }, 470f, 1.35f, 1.4f), new Enemies(false, 0, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, false), new Lasers(750f, 1.5f), new Turrets(false, 0, 0f, 0f, 0f, 0f, 0f, 0f), new SmallBird(3.45f, 2.07f, -300f, 4, new MinMax(100f, 470f), 70f, 8f, 8f, 700f, 2f), new BigBird(0f), new Nurses(0f, 0f, 0f, 0f, string.Empty, 0f, string.Empty, 0f), new Garbage(0f, 0f, 0f, 0f, string.Empty, 0f, 0f, new MinMax(0f, 1f), new string[0]), new Heart(0f, 0, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, 0f, new string[0], new string[0])));
				list.Add(new State(0.25f, new Pattern[1][] { new Pattern[0] }, States.HouseDeath, new Floating(2.8f, 250f, -120f, new MinMax(0.2f, 2.2f)), new Feathers(new string[1] { "P:10" }, 8, 680f, 29f, 0.33f, 0.33f, 1.7f), new Eggs(new string[5] { "P:1,D:1,P:2", "D:0.5,P:2", "P:1,D:0.1,P:1", "P:2,D:1,P:1", "D:0.7,P:2" }, 470f, 1.35f, 1.4f), new Enemies(false, 0, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, false), new Lasers(750f, 1.5f), new Turrets(false, 0, 0f, 0f, 0f, 0f, 0f, 0f), new SmallBird(3.45f, 2.07f, -300f, 4, new MinMax(100f, 470f), 70f, 8f, 8f, 700f, 2f), new BigBird(0f), new Nurses(0f, 0f, 0f, 0f, string.Empty, 0f, string.Empty, 0f), new Garbage(0f, 0f, 0f, 0f, string.Empty, 0f, 0f, new MinMax(0f, 1f), new string[0]), new Heart(0f, 0, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, 0f, new string[0], new string[0])));
				break;
			case Level.Mode.Normal:
				hp = 2400;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Eggs,
					Pattern.Lasers
				} }, States.Main, new Floating(2.4f, 360f, -120f, new MinMax(0.5f, 1.5f)), new Feathers(new string[1] { "P:30" }, 9, 750f, 23f, 0.25f, 0.25f, 3.4f), new Eggs(new string[3] { "D:1,P:1,D:1,P:2,D:1,P:1", "P:1,D:1,P:1,D:1,P:2", "D:1,P:2,D:1,P:1,D:1,P:1" }, 540f, 1.2f, 3f), new Enemies(false, 3, 1f, 8, 270f, 100f, 2f, 600f, 1.6f, 2.1f, 5f, 0f, false), new Lasers(900f, 1f), new Turrets(false, 8, 3f, 800f, 3f, 6f, 100f, 2f), new SmallBird(4.5f, 2.3f, -300f, 5, new MinMax(100f, 470f), 80f, 3.7f, 7f, 700f, 2f), new BigBird(7.4f), new Nurses(500f, 450f, 150f, 0.8f, "R,R,R,P,R,R,R,R,P", 3f, "6,2,4,2,4,2", 4f), new Garbage(210f, 1020f, 160f, 200f, "4,5", 0.5f, 1f, new MinMax(1.5f, 3.3f), new string[4] { "F,B,P,F,A,B", "P,F,B,A,A,F", "B,F,A,P,F,A", "F,P,B,F,A,B" }), new Heart(425f, 1, new MinMax(1.8f, 3.4f), new MinMax(0f, 60f), 450f, 500f, new string[8] { "0.6", "1.1", "0.5", "1.5", "0.7", "1.6", "0.8", "1.4" }, new string[1] { "3" })));
				list.Add(new State(0.9f, new Pattern[1][] { new Pattern[9]
				{
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Lasers,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Lasers
				} }, States.Generic, new Floating(2.4f, 360f, -120f, new MinMax(0.5f, 1.5f)), new Feathers(new string[1] { "P:30" }, 9, 750f, 23f, 0.25f, 0.25f, 3.4f), new Eggs(new string[4] { "D:0.1,P:1", "D:0.6,P:2", "D:0.5,P:1", "D:0.2,P:2" }, 540f, 1.3f, 1f), new Enemies(true, 4, 0.8f, 8, 300f, 100f, 2f, 600f, 1f, 10f, 4.5f, 1.5f, false), new Lasers(900f, 1f), new Turrets(false, 8, 3f, 800f, 3f, 6f, 100f, 2f), new SmallBird(4.5f, 2.3f, -300f, 5, new MinMax(100f, 470f), 80f, 3.7f, 7f, 700f, 2f), new BigBird(7.4f), new Nurses(500f, 450f, 150f, 0.8f, "R,R,R,P,R,R,R,R,P", 3f, "6,2,4,2,4,2", 4f), new Garbage(210f, 1020f, 160f, 200f, "4,5", 0.5f, 1f, new MinMax(1.5f, 3.3f), new string[4] { "F,B,P,F,A,B", "P,F,B,A,A,F", "B,F,A,P,F,A", "F,P,B,F,A,B" }), new Heart(425f, 1, new MinMax(1.8f, 3.4f), new MinMax(0f, 60f), 450f, 500f, new string[8] { "0.6", "1.1", "0.5", "1.5", "0.7", "1.6", "0.8", "1.4" }, new string[1] { "3" })));
				list.Add(new State(0.75f, new Pattern[1][] { new Pattern[1] }, States.Whistle, new Floating(2.6f, 250f, -120f, new MinMax(1.2f, 2.2f)), new Feathers(new string[1] { "P:30" }, 9, 750f, 23f, 0.25f, 0.25f, 3.4f), new Eggs(new string[2] { "P:1", "D:0.5,P:1" }, 540f, 1.5f, 1f), new Enemies(true, 4, 1f, 8, 270f, 100f, 2f, 600f, 1f, 10f, 4.5f, 4f, false), new Lasers(900f, 1f), new Turrets(false, 8, 3f, 800f, 3f, 6f, 100f, 2f), new SmallBird(4.5f, 2.3f, -300f, 5, new MinMax(100f, 470f), 80f, 3.7f, 7f, 700f, 2f), new BigBird(7.4f), new Nurses(500f, 450f, 150f, 0.8f, "R,R,R,P,R,R,R,R,P", 3f, "6,2,4,2,4,2", 4f), new Garbage(210f, 1020f, 160f, 200f, "4,5", 0.5f, 1f, new MinMax(1.5f, 3.3f), new string[4] { "F,B,P,F,A,B", "P,F,B,A,A,F", "B,F,A,P,F,A", "F,P,B,F,A,B" }), new Heart(425f, 1, new MinMax(1.8f, 3.4f), new MinMax(0f, 60f), 450f, 500f, new string[8] { "0.6", "1.1", "0.5", "1.5", "0.7", "1.6", "0.8", "1.4" }, new string[1] { "3" })));
				list.Add(new State(0.52f, new Pattern[1][] { new Pattern[0] }, States.HouseDeath, new Floating(2.6f, 250f, -120f, new MinMax(1.2f, 2.2f)), new Feathers(new string[1] { "P:30" }, 9, 750f, 23f, 0.25f, 0.25f, 3.4f), new Eggs(new string[2] { "P:1", "D:0.5,P:1" }, 540f, 1.5f, 1f), new Enemies(false, 0, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, false), new Lasers(900f, 1f), new Turrets(false, 8, 3f, 800f, 3f, 6f, 100f, 2f), new SmallBird(4.5f, 2.3f, -300f, 5, new MinMax(100f, 470f), 80f, 3.7f, 7f, 700f, 2f), new BigBird(7.4f), new Nurses(500f, 450f, 150f, 0.8f, "R,R,R,P,R,R,R,R,P", 3f, "6,2,4,2,4,2", 4f), new Garbage(210f, 1020f, 160f, 200f, "4,5", 0.5f, 1f, new MinMax(1.5f, 3.3f), new string[4] { "F,B,P,F,A,B", "P,F,B,A,A,F", "B,F,A,P,F,A", "F,P,B,F,A,B" }), new Heart(425f, 1, new MinMax(1.8f, 3.4f), new MinMax(0f, 60f), 450f, 500f, new string[8] { "0.6", "1.1", "0.5", "1.5", "0.7", "1.6", "0.8", "1.4" }, new string[1] { "3" })));
				list.Add(new State(0.29f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Garbage,
					Pattern.Heart
				} }, States.BirdRevival, new Floating(2.6f, 250f, -120f, new MinMax(1.2f, 2.2f)), new Feathers(new string[1] { "P:30" }, 9, 750f, 23f, 0.25f, 0.25f, 3.4f), new Eggs(new string[2] { "P:1", "D:0.5,P:1" }, 540f, 1.5f, 1f), new Enemies(false, 4, 0.8f, 8, 300f, 100f, 2f, 600f, 1f, 10f, 5f, 1.5f, true), new Lasers(900f, 1f), new Turrets(false, 8, 3f, 800f, 3f, 6f, 100f, 2f), new SmallBird(4.5f, 2.3f, -300f, 5, new MinMax(100f, 470f), 80f, 3.7f, 7f, 700f, 2f), new BigBird(7.4f), new Nurses(500f, 450f, 150f, 0.8f, "R,R,R,P,R,R,R,R,P", 3f, "6,2,4,2,4,2", 4f), new Garbage(210f, 1020f, 160f, 200f, "4,5", 0.5f, 1f, new MinMax(1.5f, 3.3f), new string[4] { "F,B,P,F,A,B", "P,F,B,A,A,F", "B,F,A,P,F,A", "F,P,B,F,A,B" }), new Heart(425f, 1, new MinMax(1.8f, 3.4f), new MinMax(0f, 60f), 450f, 500f, new string[8] { "0.6", "1.1", "0.5", "1.5", "0.7", "1.6", "0.8", "1.4" }, new string[1] { "3" })));
				break;
			case Level.Mode.Hard:
				hp = 2800;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[19]
				{
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Lasers,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Lasers,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Lasers,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Lasers,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Eggs,
					Pattern.Lasers
				} }, States.Main, new Floating(2.4f, 360f, -120f, new MinMax(0.5f, 2.2f)), new Feathers(new string[4] { "P:15", "P:20", "P:25", "P:20" }, 10, 850f, 22f, 0.23f, 0.23f, 1.4f), new Eggs(new string[5] { "P:3", "P:1", "P:2", "P:2", "P:1" }, 500f, 1.2f, 1f), new Enemies(true, 4, 0.7f, 8, 350f, 100f, 2f, 600f, 1f, 10f, 4.1f, 2f, true), new Lasers(1050f, 0.7f), new Turrets(false, 0, 0f, 0f, 0f, 0f, 0f, 0f), new SmallBird(3.2f, 1.4f, -245f, 6, new MinMax(100f, 545f), 110f, 5f, 6.5f, 700f, 2f), new BigBird(3.7f), new Nurses(550f, 450f, 130f, 0.8f, "R,R,R,R,P,R,R,R,R,R,P", 2f, "6,2,4,2", 4f), new Garbage(210f, 1020f, 160f, 150f, "5,6", 0.2f, 1f, new MinMax(1.5f, 3.3f), new string[4] { "P,F,B,A,B,F", "B,F,A,P,A,B", "A,B,F,P,A,F", "A,P,F,B,F,A" }), new Heart(425f, 2, new MinMax(1.8f, 3.4f), new MinMax(0f, 60f), 500f, 500f, new string[13]
				{
					"0.6,0.8", "0.8,0.8", "0.5,0.5", "0.7,0.5", "0.6,0.9", "0.9,0.7", "0.7,0.7", "0.6,0.9", "0.8,0.5", "0.7,0.8",
					"0.5,0.6", "0.9,0.7", "0.5,0.9"
				}, new string[1] { "3,3" })));
				list.Add(new State(0.75f, new Pattern[1][] { new Pattern[1] }, States.Whistle, new Floating(2.4f, 250f, -120f, new MinMax(1.3f, 2.2f)), new Feathers(new string[4] { "P:15", "P:20", "P:25", "P:20" }, 10, 850f, 22f, 0.23f, 0.23f, 1.4f), new Eggs(new string[5] { "P:3", "P:1", "P:2", "P:2", "P:1" }, 500f, 1.2f, 1f), new Enemies(true, 4, 0.7f, 8, 350f, 100f, 2f, 600f, 1f, 10f, 4.1f, 2f, true), new Lasers(1050f, 0.7f), new Turrets(false, 0, 0f, 0f, 0f, 0f, 0f, 0f), new SmallBird(3.2f, 1.4f, -245f, 6, new MinMax(100f, 545f), 110f, 5f, 6.5f, 700f, 2f), new BigBird(3.7f), new Nurses(550f, 450f, 130f, 0.8f, "R,R,R,R,P,R,R,R,R,R,P", 2f, "6,2,4,2", 4f), new Garbage(210f, 1020f, 160f, 150f, "5,6", 0.2f, 1f, new MinMax(1.5f, 3.3f), new string[4] { "P,F,B,A,B,F", "B,F,A,P,A,B", "A,B,F,P,A,F", "A,P,F,B,F,A" }), new Heart(425f, 2, new MinMax(1.8f, 3.4f), new MinMax(0f, 60f), 500f, 500f, new string[13]
				{
					"0.6,0.8", "0.8,0.8", "0.5,0.5", "0.7,0.5", "0.6,0.9", "0.9,0.7", "0.7,0.7", "0.6,0.9", "0.8,0.5", "0.7,0.8",
					"0.5,0.6", "0.9,0.7", "0.5,0.9"
				}, new string[1] { "3,3" })));
				list.Add(new State(0.5f, new Pattern[1][] { new Pattern[0] }, States.HouseDeath, new Floating(2.4f, 250f, -120f, new MinMax(1.3f, 2.2f)), new Feathers(new string[4] { "P:15", "P:20", "P:25", "P:20" }, 10, 850f, 22f, 0.23f, 0.23f, 1.4f), new Eggs(new string[5] { "P:3", "P:1", "P:2", "P:2", "P:1" }, 500f, 1.2f, 1f), new Enemies(false, 0, 0f, 0, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, false), new Lasers(1050f, 0.7f), new Turrets(false, 0, 0f, 0f, 0f, 0f, 0f, 0f), new SmallBird(3.2f, 1.4f, -245f, 6, new MinMax(100f, 545f), 110f, 5f, 6.5f, 700f, 2f), new BigBird(3.7f), new Nurses(550f, 450f, 130f, 0.8f, "R,R,R,R,P,R,R,R,R,R,P", 2f, "6,2,4,2", 4f), new Garbage(210f, 1020f, 160f, 150f, "5,6", 0.2f, 1f, new MinMax(1.5f, 3.3f), new string[4] { "P,F,B,A,B,F", "B,F,A,P,A,B", "A,B,F,P,A,F", "A,P,F,B,F,A" }), new Heart(425f, 2, new MinMax(1.8f, 3.4f), new MinMax(0f, 60f), 500f, 500f, new string[13]
				{
					"0.6,0.8", "0.8,0.8", "0.5,0.5", "0.7,0.5", "0.6,0.9", "0.9,0.7", "0.7,0.7", "0.6,0.9", "0.8,0.5", "0.7,0.8",
					"0.5,0.6", "0.9,0.7", "0.5,0.9"
				}, new string[1] { "3,3" })));
				list.Add(new State(0.31f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Heart,
					Pattern.Garbage
				} }, States.BirdRevival, new Floating(2.4f, 250f, -120f, new MinMax(1.3f, 2.2f)), new Feathers(new string[4] { "P:15", "P:20", "P:25", "P:20" }, 10, 850f, 22f, 0.23f, 0.23f, 1.4f), new Eggs(new string[5] { "P:3", "P:1", "P:2", "P:2", "P:1" }, 500f, 1.2f, 1f), new Enemies(false, 4, 0.7f, 8, 350f, 100f, 2f, 600f, 1f, 10f, 4.5f, 2f, true), new Lasers(1050f, 0.7f), new Turrets(false, 0, 0f, 0f, 0f, 0f, 0f, 0f), new SmallBird(3.2f, 1.4f, -245f, 6, new MinMax(100f, 545f), 110f, 5f, 6.5f, 700f, 2f), new BigBird(3.7f), new Nurses(550f, 450f, 130f, 0.8f, "R,R,R,R,P,R,R,R,R,R,P", 2f, "6,2,4,2", 4f), new Garbage(210f, 1020f, 160f, 150f, "5,6", 0.2f, 1f, new MinMax(1.5f, 3.3f), new string[4] { "P,F,B,A,B,F", "B,F,A,P,A,B", "A,B,F,P,A,F", "A,P,F,B,F,A" }), new Heart(425f, 2, new MinMax(1.8f, 3.4f), new MinMax(0f, 60f), 500f, 500f, new string[13]
				{
					"0.6,0.8", "0.8,0.8", "0.5,0.5", "0.7,0.5", "0.6,0.9", "0.9,0.7", "0.7,0.7", "0.6,0.9", "0.8,0.5", "0.7,0.8",
					"0.5,0.6", "0.9,0.7", "0.5,0.9"
				}, new string[1] { "3,3" })));
				break;
			}
			return new FlyingBird(hp, goalTimes, list.ToArray());
		}
	}

	public class FlyingBlimp : AbstractLevelProperties<FlyingBlimp.State, FlyingBlimp.Pattern, FlyingBlimp.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected FlyingBlimp properties { get; private set; }

			public virtual void LevelInit(FlyingBlimp properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			Moon = 2,
			Sagittarius = 3,
			Taurus = 4,
			Gemini = 5,
			SagOrGem = 6
		}

		public enum Pattern
		{
			Dash = 0,
			Tornado = 1,
			Shoot = 2,
			Uninitialized = 3
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Move move;

			public readonly DashSummon dashSummon;

			public readonly Enemy enemy;

			public readonly Tornado tornado;

			public readonly Shoot shoot;

			public readonly Morph morph;

			public readonly Stars stars;

			public readonly Sagittarius sagittarius;

			public readonly Taurus taurus;

			public readonly Gemini gemini;

			public readonly UFO uFO;

			public readonly Gear gear;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Move move, DashSummon dashSummon, Enemy enemy, Tornado tornado, Shoot shoot, Morph morph, Stars stars, Sagittarius sagittarius, Taurus taurus, Gemini gemini, UFO uFO, Gear gear)
				: base(healthTrigger, patterns, stateName)
			{
				this.move = move;
				this.dashSummon = dashSummon;
				this.enemy = enemy;
				this.tornado = tornado;
				this.shoot = shoot;
				this.morph = morph;
				this.stars = stars;
				this.sagittarius = sagittarius;
				this.taurus = taurus;
				this.gemini = gemini;
				this.uFO = uFO;
				this.gear = gear;
			}
		}

		public class Move : AbstractLevelPropertyGroup
		{
			public readonly float pathSpeed;

			public readonly MinMax initalAttackDelayRange;

			public Move(float pathSpeed, MinMax initalAttackDelayRange)
			{
				this.pathSpeed = pathSpeed;
				this.initalAttackDelayRange = initalAttackDelayRange;
			}
		}

		public class DashSummon : AbstractLevelPropertyGroup
		{
			public readonly string[] patternString;

			public readonly float hold;

			public readonly float dashSpeed;

			public readonly float reeentryDelay;

			public readonly float summonSpeed;

			public readonly float summonHesitate;

			public DashSummon(string[] patternString, float hold, float dashSpeed, float reeentryDelay, float summonSpeed, float summonHesitate)
			{
				this.patternString = patternString;
				this.hold = hold;
				this.dashSpeed = dashSpeed;
				this.reeentryDelay = reeentryDelay;
				this.summonSpeed = summonSpeed;
				this.summonHesitate = summonHesitate;
			}
		}

		public class Enemy : AbstractLevelPropertyGroup
		{
			public readonly bool active;

			public readonly int hp;

			public readonly float speed;

			public readonly float shotDelay;

			public readonly MinMax stopDistance;

			public readonly float stringDelay;

			public readonly string[] spawnString;

			public readonly string[] typeString;

			public readonly MinMax spreadAngle;

			public readonly int numBullets;

			public readonly float ASpeed;

			public readonly float BSpeed;

			public readonly MinMax APinkOccurance;

			public Enemy(bool active, int hp, float speed, float shotDelay, MinMax stopDistance, float stringDelay, string[] spawnString, string[] typeString, MinMax spreadAngle, int numBullets, float ASpeed, float BSpeed, MinMax APinkOccurance)
			{
				this.active = active;
				this.hp = hp;
				this.speed = speed;
				this.shotDelay = shotDelay;
				this.stopDistance = stopDistance;
				this.stringDelay = stringDelay;
				this.spawnString = spawnString;
				this.typeString = typeString;
				this.spreadAngle = spreadAngle;
				this.numBullets = numBullets;
				this.ASpeed = ASpeed;
				this.BSpeed = BSpeed;
				this.APinkOccurance = APinkOccurance;
			}
		}

		public class Tornado : AbstractLevelPropertyGroup
		{
			public readonly float moveSpeed;

			public readonly float homingSpeed;

			public readonly float loopDuration;

			public readonly float hesitateAfterAttack;

			public Tornado(float moveSpeed, float homingSpeed, float loopDuration, float hesitateAfterAttack)
			{
				this.moveSpeed = moveSpeed;
				this.homingSpeed = homingSpeed;
				this.loopDuration = loopDuration;
				this.hesitateAfterAttack = hesitateAfterAttack;
			}
		}

		public class Shoot : AbstractLevelPropertyGroup
		{
			public readonly float speedMin;

			public readonly float speedMax;

			public readonly float accelerationTime;

			public readonly MinMax hesitateAfterAttackRange;

			public Shoot(float speedMin, float speedMax, float accelerationTime, MinMax hesitateAfterAttackRange)
			{
				this.speedMin = speedMin;
				this.speedMax = speedMax;
				this.accelerationTime = accelerationTime;
				this.hesitateAfterAttackRange = hesitateAfterAttackRange;
			}
		}

		public class Morph : AbstractLevelPropertyGroup
		{
			public readonly float crazyAHold;

			public readonly float crazyBHold;

			public Morph(float crazyAHold, float crazyBHold)
			{
				this.crazyAHold = crazyAHold;
				this.crazyBHold = crazyBHold;
			}
		}

		public class Stars : AbstractLevelPropertyGroup
		{
			public readonly MinMax speedX;

			public readonly float speedY;

			public readonly float sineSize;

			public readonly float delay;

			public readonly string[] typeString;

			public readonly string[] positionString;

			public Stars(MinMax speedX, float speedY, float sineSize, float delay, string[] typeString, string[] positionString)
			{
				this.speedX = speedX;
				this.speedY = speedY;
				this.sineSize = sineSize;
				this.delay = delay;
				this.typeString = typeString;
				this.positionString = positionString;
			}
		}

		public class Sagittarius : AbstractLevelPropertyGroup
		{
			public readonly int arrowHP;

			public readonly int movementSpeed;

			public readonly MinMax attackDelayRange;

			public readonly float arrowInitialSpeed;

			public readonly float arrowWarning;

			public readonly MinMax homingSpreadAngle;

			public readonly float homingDelay;

			public readonly float homingSpeed;

			public readonly float homingRotation;

			public readonly MinMax homingDurationRange;

			public Sagittarius(int arrowHP, int movementSpeed, MinMax attackDelayRange, float arrowInitialSpeed, float arrowWarning, MinMax homingSpreadAngle, float homingDelay, float homingSpeed, float homingRotation, MinMax homingDurationRange)
			{
				this.arrowHP = arrowHP;
				this.movementSpeed = movementSpeed;
				this.attackDelayRange = attackDelayRange;
				this.arrowInitialSpeed = arrowInitialSpeed;
				this.arrowWarning = arrowWarning;
				this.homingSpreadAngle = homingSpreadAngle;
				this.homingDelay = homingDelay;
				this.homingSpeed = homingSpeed;
				this.homingRotation = homingRotation;
				this.homingDurationRange = homingDurationRange;
			}
		}

		public class Taurus : AbstractLevelPropertyGroup
		{
			public readonly MinMax attackDelayRange;

			public readonly float movementSpeed;

			public Taurus(MinMax attackDelayRange, float movementSpeed)
			{
				this.attackDelayRange = attackDelayRange;
				this.movementSpeed = movementSpeed;
			}
		}

		public class Gemini : AbstractLevelPropertyGroup
		{
			public readonly MinMax spawnerDelay;

			public readonly float spawnerSpeed;

			public readonly float bulletDelay;

			public readonly float rotationSpeed;

			public readonly float bulletSpeed;

			public Gemini(MinMax spawnerDelay, float spawnerSpeed, float bulletDelay, float rotationSpeed, float bulletSpeed)
			{
				this.spawnerDelay = spawnerDelay;
				this.spawnerSpeed = spawnerSpeed;
				this.bulletDelay = bulletDelay;
				this.rotationSpeed = rotationSpeed;
				this.bulletSpeed = bulletSpeed;
			}
		}

		public class UFO : AbstractLevelPropertyGroup
		{
			public readonly float UFOHP;

			public readonly float UFOSpeed;

			public readonly float UFOProximityA;

			public readonly float UFOProximityB;

			public readonly float beamDuration;

			public readonly float UFODelay;

			public readonly string[] UFOString;

			public readonly float UFOWarningBeamDuration;

			public readonly float UFOInitialDelay;

			public readonly float moonATKAnticipation;

			public readonly float moonATKDuration;

			public readonly float moonWaitForNextATK;

			public readonly bool invincibility;

			public UFO(float UFOHP, float UFOSpeed, float UFOProximityA, float UFOProximityB, float beamDuration, float UFODelay, string[] UFOString, float UFOWarningBeamDuration, float UFOInitialDelay, float moonATKAnticipation, float moonATKDuration, float moonWaitForNextATK, bool invincibility)
			{
				this.UFOHP = UFOHP;
				this.UFOSpeed = UFOSpeed;
				this.UFOProximityA = UFOProximityA;
				this.UFOProximityB = UFOProximityB;
				this.beamDuration = beamDuration;
				this.UFODelay = UFODelay;
				this.UFOString = UFOString;
				this.UFOWarningBeamDuration = UFOWarningBeamDuration;
				this.UFOInitialDelay = UFOInitialDelay;
				this.moonATKAnticipation = moonATKAnticipation;
				this.moonATKDuration = moonATKDuration;
				this.moonWaitForNextATK = moonWaitForNextATK;
				this.invincibility = invincibility;
			}
		}

		public class Gear : AbstractLevelPropertyGroup
		{
			public readonly int parryCount;

			public readonly float bounceSpeed;

			public readonly float bounceHeight;

			public Gear(int parryCount, float bounceSpeed, float bounceHeight)
			{
				this.parryCount = parryCount;
				this.bounceSpeed = bounceSpeed;
				this.bounceHeight = bounceHeight;
			}
		}

		public FlyingBlimp(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 2200f;
				timeline.events.Add(new Level.Timeline.Event("Taurus", 0.85f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.62f));
				timeline.events.Add(new Level.Timeline.Event("Gemini", 0.46f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.23f));
				break;
			case Level.Mode.Normal:
				timeline.health = 2600f;
				timeline.events.Add(new Level.Timeline.Event("Taurus", 0.95f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.77f));
				timeline.events.Add(new Level.Timeline.Event("SagOrGem", 0.64f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.46f));
				timeline.events.Add(new Level.Timeline.Event("Moon", 0.34f));
				break;
			case Level.Mode.Hard:
				timeline.health = 3000f;
				timeline.events.Add(new Level.Timeline.Event("Gemini", 0.95f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.8f));
				timeline.events.Add(new Level.Timeline.Event("Sagittarius", 0.67f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.51f));
				timeline.events.Add(new Level.Timeline.Event("Moon", 0.39f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "D":
				return Pattern.Dash;
			case "T":
				return Pattern.Tornado;
			case "S":
				return Pattern.Shoot;
			default:
				Debug.LogError("Pattern FlyingBlimp.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static FlyingBlimp GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 2200;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] { Pattern.Shoot } }, States.Main, new Move(4.2f, new MinMax(1.5f, 4f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 700f, 0.4f, new MinMax(10f, 80f), 4.6f, new string[1] { "50,250,450,150,550,100,300,500,200,600" }, new string[1] { "A,A" }, new MinMax(0f, 90f), 3, 425f, 475f, new MinMax(1f, 1f)), new Tornado(600f, 0.75f, 1f, 3.4f), new Shoot(400f, 1600f, 1300f, new MinMax(1.5f, 2f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(1f, 1f), 1f, 1f, 1f, new string[1] { "A,B,C" }, new string[1] { "1,100" }), new Sagittarius(8, 3, new MinMax(3f, 6f), 450f, 0.5f, new MinMax(0f, 90f), 1.5f, 470f, 2.3f, new MinMax(0f, 1f)), new Taurus(new MinMax(3.5f, 6f), 3.4f), new Gemini(new MinMax(2f, 4f), 1f, 5f, 0.18f, 300f), new UFO(1f, 1f, 1f, 1f, 11f, 1f, new string[1] { "A,B" }, 1f, 1f, 1f, 1f, 1f, false), new Gear(2, -255f, 1000f)));
				list.Add(new State(0.85f, new Pattern[1][] { new Pattern[0] }, States.Taurus, new Move(4.2f, new MinMax(1.5f, 4f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 700f, 0.4f, new MinMax(10f, 80f), 4.4f, new string[1] { "50,250,450,150,550,100,300,500,200,600" }, new string[1] { "A,A,B" }, new MinMax(0f, 90f), 3, 425f, 475f, new MinMax(1f, 2f)), new Tornado(600f, 0.75f, 1f, 3.4f), new Shoot(400f, 1600f, 1300f, new MinMax(1.5f, 2f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(1f, 1f), 1f, 1f, 1f, new string[1] { "A,B,C" }, new string[1] { "1,100" }), new Sagittarius(8, 3, new MinMax(3f, 6f), 450f, 0.5f, new MinMax(0f, 90f), 1.5f, 470f, 2.3f, new MinMax(0f, 1f)), new Taurus(new MinMax(3.5f, 6f), 3.4f), new Gemini(new MinMax(2f, 4f), 1f, 5f, 0.18f, 300f), new UFO(1f, 1f, 1f, 1f, 11f, 1f, new string[1] { "A,B" }, 1f, 1f, 1f, 1f, 1f, false), new Gear(2, -255f, 1000f)));
				list.Add(new State(0.62f, new Pattern[1][] { new Pattern[3]
				{
					Pattern.Tornado,
					Pattern.Shoot,
					Pattern.Shoot
				} }, States.Generic, new Move(4.2f, new MinMax(1.5f, 4f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 700f, 0.4f, new MinMax(10f, 80f), 4.4f, new string[1] { "50,250,450,150,550,100,300,500,200,600" }, new string[1] { "A" }, new MinMax(0f, 90f), 3, 425f, 475f, new MinMax(1f, 2f)), new Tornado(600f, 0.75f, 1f, 3.4f), new Shoot(400f, 1600f, 1300f, new MinMax(1.5f, 2f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(1f, 1f), 1f, 1f, 1f, new string[1] { "A,B,C" }, new string[1] { "1,100" }), new Sagittarius(8, 3, new MinMax(3f, 6f), 450f, 0.5f, new MinMax(0f, 90f), 1.5f, 470f, 2.3f, new MinMax(0f, 1f)), new Taurus(new MinMax(3.5f, 6f), 3.4f), new Gemini(new MinMax(2f, 4f), 1f, 5f, 0.18f, 300f), new UFO(1f, 1f, 1f, 1f, 11f, 1f, new string[1] { "A,B" }, 1f, 1f, 1f, 1f, 1f, false), new Gear(2, -255f, 1000f)));
				list.Add(new State(0.46f, new Pattern[1][] { new Pattern[0] }, States.Gemini, new Move(4.2f, new MinMax(1.5f, 4f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 700f, 0.4f, new MinMax(0f, -20f), 4.2f, new string[1] { "50,250,450,150,550,100,300,500,200,600" }, new string[1] { "A,A" }, new MinMax(0f, 90f), 3, 425f, 475f, new MinMax(1f, 2f)), new Tornado(600f, 0.75f, 1f, 3.4f), new Shoot(400f, 1600f, 1300f, new MinMax(1.5f, 2f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(1f, 1f), 1f, 1f, 1f, new string[1] { "A,B,C" }, new string[1] { "1,100" }), new Sagittarius(8, 3, new MinMax(3f, 6f), 450f, 0.5f, new MinMax(0f, 90f), 1.5f, 470f, 2.3f, new MinMax(0f, 1f)), new Taurus(new MinMax(3.5f, 6f), 3.4f), new Gemini(new MinMax(2f, 4f), 1f, 5f, 0.18f, 300f), new UFO(1f, 1f, 1f, 1f, 11f, 1f, new string[1] { "A,B" }, 1f, 1f, 1f, 1f, 1f, false), new Gear(2, -255f, 1000f)));
				list.Add(new State(0.23f, new Pattern[1][] { new Pattern[7]
				{
					Pattern.Shoot,
					Pattern.Shoot,
					Pattern.Tornado,
					Pattern.Shoot,
					Pattern.Shoot,
					Pattern.Shoot,
					Pattern.Tornado
				} }, States.Generic, new Move(4.2f, new MinMax(1.5f, 4f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 900f, 0.4f, new MinMax(10f, 80f), 2.5f, new string[1] { "50,250,450,150,550,100,300,500,200,600" }, new string[1] { "A" }, new MinMax(0f, 90f), 3, 425f, 475f, new MinMax(2f, 3f)), new Tornado(600f, 0.75f, 1f, 3.4f), new Shoot(400f, 1600f, 1300f, new MinMax(1.5f, 2f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(1f, 1f), 1f, 1f, 1f, new string[1] { "A,B,C" }, new string[1] { "1,100" }), new Sagittarius(8, 3, new MinMax(3f, 6f), 450f, 0.5f, new MinMax(0f, 90f), 1.5f, 470f, 2.3f, new MinMax(0f, 1f)), new Taurus(new MinMax(3.5f, 6f), 3.4f), new Gemini(new MinMax(2f, 4f), 1f, 5f, 0.18f, 300f), new UFO(1f, 1f, 1f, 1f, 11f, 1f, new string[1] { "A,B" }, 1f, 1f, 1f, 1f, 1f, false), new Gear(2, -255f, 1000f)));
				break;
			case Level.Mode.Normal:
				hp = 2600;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] { Pattern.Shoot } }, States.Main, new Move(5f, new MinMax(1.3f, 3f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 750f, 0.4f, new MinMax(10f, 80f), 4.2f, new string[1] { "50,250,450,150,550,100,300,500,200,600" }, new string[1] { "A,A,B" }, new MinMax(0f, 90f), 4, 525f, 475f, new MinMax(1f, 1f)), new Tornado(600f, 0.9f, 1f, 3.3f), new Shoot(400f, 1600f, 1300f, new MinMax(1.5f, 1.8f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(300f, 800f), 5f, 5f, 1.35f, new string[2] { "A,B,C,A,C,A,A,B,C,C,P,A,B,C,B,A,A", "C,B,A,C,A,A,B,C,A,B,A,P,C,C,B,A,A" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "200,400,600,50,300,550" }), new Sagittarius(10, 3, new MinMax(3f, 6f), 705f, 0.5f, new MinMax(0f, 90f), 1f, 447f, 2.3f, new MinMax(3f, 5f)), new Taurus(new MinMax(2.6f, 5.5f), 4f), new Gemini(new MinMax(2f, 3.5f), 1f, 5f, 0.22f, 300f), new UFO(9999f, 330f, 115f, 385f, 0.5f, 2f, new string[5] { "A,B,A,A,B,B,A", "A,A,B,A,A,B,A", "B,A,B,A,A,A,B", "A,B,B,A,A,B,A", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 9f, 2f, false), new Gear(2, -255f, 1100f)));
				list.Add(new State(0.95f, new Pattern[1][] { new Pattern[0] }, States.Taurus, new Move(5f, new MinMax(1.3f, 3f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 750f, 0.4f, new MinMax(10f, 70f), 4f, new string[1] { "50,250,450,150,550,100,300,500,200,600" }, new string[1] { "A,A,A,A,A,A,A,A,A,A,A,A,A,A,A,A,A,A,A,A,A,A,A,A" }, new MinMax(0f, 90f), 3, 525f, 475f, new MinMax(1f, 2f)), new Tornado(600f, 0.9f, 1f, 3.3f), new Shoot(400f, 1600f, 1300f, new MinMax(1.5f, 1.8f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(300f, 800f), 5f, 5f, 1.35f, new string[2] { "A,B,C,A,C,A,A,B,C,C,P,A,B,C,B,A,A", "C,B,A,C,A,A,B,C,A,B,A,P,C,C,B,A,A" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "200,400,600,50,300,550" }), new Sagittarius(10, 3, new MinMax(3f, 6f), 705f, 0.5f, new MinMax(0f, 90f), 1f, 447f, 2.3f, new MinMax(3f, 5f)), new Taurus(new MinMax(2.6f, 5.5f), 4f), new Gemini(new MinMax(2f, 3.5f), 1f, 5f, 0.22f, 300f), new UFO(9999f, 330f, 115f, 385f, 0.5f, 2f, new string[5] { "A,B,A,A,B,B,A", "A,A,B,A,A,B,A", "B,A,B,A,A,A,B", "A,B,B,A,A,B,A", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 9f, 2f, false), new Gear(2, -255f, 1100f)));
				list.Add(new State(0.77f, new Pattern[1][] { new Pattern[7]
				{
					Pattern.Tornado,
					Pattern.Shoot,
					Pattern.Shoot,
					Pattern.Shoot,
					Pattern.Tornado,
					Pattern.Shoot,
					Pattern.Shoot
				} }, States.Generic, new Move(5f, new MinMax(1.3f, 3f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 750f, 0.4f, new MinMax(10f, 70f), 3.8f, new string[1] { "50,250,450,150,550,100,300,500,200,600" }, new string[1] { "A,B,A,A,B,B,A,A,A,B,A,A,B,A,B" }, new MinMax(0f, 90f), 4, 525f, 475f, new MinMax(1f, 2f)), new Tornado(600f, 0.9f, 1f, 3.3f), new Shoot(400f, 1600f, 1300f, new MinMax(1.5f, 1.8f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(300f, 800f), 5f, 5f, 1.35f, new string[2] { "A,B,C,A,C,A,A,B,C,C,P,A,B,C,B,A,A", "C,B,A,C,A,A,B,C,A,B,A,P,C,C,B,A,A" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "200,400,600,50,300,550" }), new Sagittarius(10, 3, new MinMax(3f, 6f), 705f, 0.5f, new MinMax(0f, 90f), 1f, 447f, 2.3f, new MinMax(3f, 5f)), new Taurus(new MinMax(2.6f, 5.5f), 4f), new Gemini(new MinMax(2f, 3.5f), 1f, 5f, 0.22f, 300f), new UFO(9999f, 330f, 115f, 385f, 0.5f, 2f, new string[5] { "A,B,A,A,B,B,A", "A,A,B,A,A,B,A", "B,A,B,A,A,A,B", "A,B,B,A,A,B,A", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 9f, 2f, false), new Gear(2, -255f, 1100f)));
				list.Add(new State(0.64f, new Pattern[1][] { new Pattern[0] }, States.SagOrGem, new Move(5f, new MinMax(1.3f, 3f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 750f, 0.4f, new MinMax(70f, 100f), 4f, new string[1] { "50,250,450,150,550,100,300,500,200,600" }, new string[1] { "A,A" }, new MinMax(0f, 90f), 4, 525f, 475f, new MinMax(1f, 2f)), new Tornado(600f, 0.9f, 1f, 3.3f), new Shoot(400f, 1600f, 1300f, new MinMax(1.5f, 1.8f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(300f, 800f), 5f, 5f, 1.35f, new string[2] { "A,B,C,A,C,A,A,B,C,C,P,A,B,C,B,A,A", "C,B,A,C,A,A,B,C,A,B,A,P,C,C,B,A,A" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "200,400,600,50,300,550" }), new Sagittarius(10, 3, new MinMax(3f, 6f), 705f, 0.5f, new MinMax(0f, 90f), 1f, 447f, 2.3f, new MinMax(3f, 5f)), new Taurus(new MinMax(2.6f, 5.5f), 4f), new Gemini(new MinMax(2f, 3.5f), 1f, 5f, 0.22f, 300f), new UFO(9999f, 330f, 115f, 385f, 0.5f, 2f, new string[5] { "A,B,A,A,B,B,A", "A,A,B,A,A,B,A", "B,A,B,A,A,A,B", "A,B,B,A,A,B,A", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 9f, 2f, false), new Gear(2, -255f, 1100f)));
				list.Add(new State(0.46f, new Pattern[1][] { new Pattern[5]
				{
					Pattern.Shoot,
					Pattern.Tornado,
					Pattern.Shoot,
					Pattern.Shoot,
					Pattern.Tornado
				} }, States.Generic, new Move(5f, new MinMax(1.3f, 3f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 750f, 0.3f, new MinMax(10f, 70f), 5.6f, new string[1] { "50-250,450-150,550-100,300-500,200-600,450" }, new string[1] { "A" }, new MinMax(0f, 90f), 4, 525f, 475f, new MinMax(1f, 2f)), new Tornado(600f, 0.9f, 1f, 3.3f), new Shoot(400f, 1600f, 1300f, new MinMax(1.5f, 1.8f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(300f, 800f), 5f, 5f, 1.35f, new string[2] { "A,B,C,A,C,A,A,B,C,C,P,A,B,C,B,A,A", "C,B,A,C,A,A,B,C,A,B,A,P,C,C,B,A,A" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "200,400,600,50,300,550" }), new Sagittarius(10, 3, new MinMax(3f, 6f), 705f, 0.5f, new MinMax(0f, 90f), 1f, 447f, 2.3f, new MinMax(3f, 5f)), new Taurus(new MinMax(2.6f, 5.5f), 4f), new Gemini(new MinMax(2f, 3.5f), 1f, 5f, 0.22f, 300f), new UFO(9999f, 330f, 115f, 385f, 0.5f, 2f, new string[5] { "A,B,A,A,B,B,A", "A,A,B,A,A,B,A", "B,A,B,A,A,A,B", "A,B,B,A,A,B,A", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 9f, 2f, false), new Gear(2, -255f, 1100f)));
				list.Add(new State(0.34f, new Pattern[1][] { new Pattern[0] }, States.Moon, new Move(5f, new MinMax(1.3f, 3f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 750f, 0.3f, new MinMax(10f, 70f), 5.6f, new string[1] { "50-250,450-150,550-100,300-500,200-600,450" }, new string[1] { "A" }, new MinMax(0f, 90f), 4, 525f, 475f, new MinMax(1f, 2f)), new Tornado(600f, 0.9f, 1f, 3.3f), new Shoot(400f, 1600f, 1300f, new MinMax(1.5f, 1.8f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(300f, 800f), 5f, 5f, 1.35f, new string[2] { "A,B,C,A,C,A,A,B,C,C,P,A,B,C,B,A,A", "C,B,A,C,A,A,B,C,A,B,A,P,C,C,B,A,A" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "200,400,600,50,300,550" }), new Sagittarius(10, 3, new MinMax(3f, 6f), 705f, 0.5f, new MinMax(0f, 90f), 1f, 447f, 2.3f, new MinMax(3f, 5f)), new Taurus(new MinMax(2.6f, 5.5f), 4f), new Gemini(new MinMax(2f, 3.5f), 1f, 5f, 0.22f, 300f), new UFO(9999f, 330f, 115f, 385f, 0.5f, 2f, new string[5] { "A,B,A,A,B,B,A", "A,A,B,A,A,B,A", "B,A,B,A,A,A,B", "A,B,B,A,A,B,A", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 9f, 2f, false), new Gear(2, -255f, 1100f)));
				break;
			case Level.Mode.Hard:
				hp = 3000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[3]
				{
					Pattern.Shoot,
					Pattern.Shoot,
					Pattern.Shoot
				} }, States.Main, new Move(5.1f, new MinMax(1.3f, 2.8f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 805f, 0.4f, new MinMax(10f, 80f), 3.6f, new string[1] { "600,200,500,300,100,550,150,450,250,50" }, new string[1] { "A,B,A" }, new MinMax(0f, 90f), 5, 575f, 525f, new MinMax(1f, 1f)), new Tornado(600f, 1.05f, 1f, 2.6f), new Shoot(450f, 1800f, 1450f, new MinMax(1.2f, 1.5f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(350f, 800f), 5f, 5f, 0.9f, new string[2] { "A,B,C,A,C,A,A,B,C,C,A,B,C,B,A,A,P", "B,C,A,A,C,B,A,B,B,C,A,B,C,C,B,A,P" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "300,600,400,50,200,500,50,550,450,150" }), new Sagittarius(10, 3, new MinMax(3f, 5.1f), 755f, 0.5f, new MinMax(0f, 90f), 1.45f, 495f, 2.5f, new MinMax(5f, 8f)), new Taurus(new MinMax(1.7f, 3.9f), 4f), new Gemini(new MinMax(1.5f, 3f), 1f, 5f, 0.3f, 300f), new UFO(9999f, 325f, 140f, 340f, 0.65f, 1.6f, new string[6] { "A,B,A,A,B,B,A", "B,A,B,B,A,A,B", "A,A,B,A,A,B,B", "B,B,A,B,A,A,A", "A,B,A,B,A,A,B", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 8f, 2f, false), new Gear(2, -255f, 1100f)));
				list.Add(new State(0.95f, new Pattern[1][] { new Pattern[0] }, States.Gemini, new Move(5.1f, new MinMax(1.3f, 2.8f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 805f, 0.4f, new MinMax(60f, 100f), 3.6f, new string[1] { "600,200,500,300,100,550,150,450,250,50" }, new string[1] { "A,A,B" }, new MinMax(0f, 90f), 5, 575f, 525f, new MinMax(1f, 2f)), new Tornado(600f, 1.05f, 1f, 2.6f), new Shoot(450f, 1800f, 1450f, new MinMax(1.2f, 1.5f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(350f, 800f), 5f, 5f, 0.9f, new string[2] { "A,B,C,A,C,A,A,B,C,C,A,B,C,B,A,A,P", "B,C,A,A,C,B,A,B,B,C,A,B,C,C,B,A,P" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "300,600,400,50,200,500,50,550,450,150" }), new Sagittarius(10, 3, new MinMax(3f, 5.1f), 755f, 0.5f, new MinMax(0f, 90f), 1.45f, 495f, 2.5f, new MinMax(5f, 8f)), new Taurus(new MinMax(1.7f, 3.9f), 4f), new Gemini(new MinMax(1.5f, 3f), 1f, 5f, 0.3f, 300f), new UFO(9999f, 325f, 140f, 340f, 0.65f, 1.6f, new string[6] { "A,B,A,A,B,B,A", "B,A,B,B,A,A,B", "A,A,B,A,A,B,B", "B,B,A,B,A,A,A", "A,B,A,B,A,A,B", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 8f, 2f, false), new Gear(2, -255f, 1100f)));
				list.Add(new State(0.8f, new Pattern[1][] { new Pattern[5]
				{
					Pattern.Shoot,
					Pattern.Tornado,
					Pattern.Shoot,
					Pattern.Shoot,
					Pattern.Tornado
				} }, States.Generic, new Move(5.1f, new MinMax(1.3f, 2.8f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 805f, 0.4f, new MinMax(50f, 100f), 3.2f, new string[1] { "600,200,500,300,100,550,150,450,250,50" }, new string[1] { "B" }, new MinMax(0f, 90f), 5, 575f, 525f, new MinMax(1f, 2f)), new Tornado(600f, 1.05f, 1f, 2.6f), new Shoot(450f, 1800f, 1450f, new MinMax(1.2f, 1.5f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(350f, 800f), 5f, 5f, 0.9f, new string[2] { "A,B,C,A,C,A,A,B,C,C,A,B,C,B,A,A,P", "B,C,A,A,C,B,A,B,B,C,A,B,C,C,B,A,P" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "300,600,400,50,200,500,50,550,450,150" }), new Sagittarius(10, 3, new MinMax(3f, 5.1f), 755f, 0.5f, new MinMax(0f, 90f), 1.45f, 495f, 2.5f, new MinMax(5f, 8f)), new Taurus(new MinMax(1.7f, 3.9f), 4f), new Gemini(new MinMax(1.5f, 3f), 1f, 5f, 0.3f, 300f), new UFO(9999f, 325f, 140f, 340f, 0.65f, 1.6f, new string[6] { "A,B,A,A,B,B,A", "B,A,B,B,A,A,B", "A,A,B,A,A,B,B", "B,B,A,B,A,A,A", "A,B,A,B,A,A,B", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 8f, 2f, false), new Gear(2, -255f, 1100f)));
				list.Add(new State(0.67f, new Pattern[1][] { new Pattern[0] }, States.Sagittarius, new Move(5.1f, new MinMax(1.3f, 2.8f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 805f, 0.4f, new MinMax(10f, 80f), 3f, new string[1] { "600,200,500,300,100,550,150,450,250,50" }, new string[1] { "A,B,A" }, new MinMax(0f, 90f), 5, 575f, 525f, new MinMax(1f, 2f)), new Tornado(600f, 1.05f, 1f, 2.6f), new Shoot(450f, 1800f, 1450f, new MinMax(1.2f, 1.5f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(350f, 800f), 5f, 5f, 0.9f, new string[2] { "A,B,C,A,C,A,A,B,C,C,A,B,C,B,A,A,P", "B,C,A,A,C,B,A,B,B,C,A,B,C,C,B,A,P" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "300,600,400,50,200,500,50,550,450,150" }), new Sagittarius(10, 3, new MinMax(3f, 5.1f), 755f, 0.5f, new MinMax(0f, 90f), 1.45f, 495f, 2.5f, new MinMax(5f, 8f)), new Taurus(new MinMax(1.7f, 3.9f), 4f), new Gemini(new MinMax(1.5f, 3f), 1f, 5f, 0.3f, 300f), new UFO(9999f, 325f, 140f, 340f, 0.65f, 1.6f, new string[6] { "A,B,A,A,B,B,A", "B,A,B,B,A,A,B", "A,A,B,A,A,B,B", "B,B,A,B,A,A,A", "A,B,A,B,A,A,B", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 8f, 2f, false), new Gear(2, -255f, 1100f)));
				list.Add(new State(0.51f, new Pattern[1][] { new Pattern[7]
				{
					Pattern.Shoot,
					Pattern.Shoot,
					Pattern.Tornado,
					Pattern.Shoot,
					Pattern.Shoot,
					Pattern.Shoot,
					Pattern.Tornado
				} }, States.Generic, new Move(5.1f, new MinMax(1.3f, 2.8f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 805f, 0.4f, new MinMax(10f, 80f), 3.2f, new string[1] { "600-200,500-300,600-400,100-550,150-450,250-50" }, new string[1] { "A,A,A,B" }, new MinMax(0f, 90f), 5, 575f, 525f, new MinMax(1f, 2f)), new Tornado(600f, 1.05f, 1f, 2.6f), new Shoot(450f, 1800f, 1450f, new MinMax(1.2f, 1.5f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(350f, 800f), 5f, 5f, 0.9f, new string[2] { "A,B,C,A,C,A,A,B,C,C,A,B,C,B,A,A,P", "B,C,A,A,C,B,A,B,B,C,A,B,C,C,B,A,P" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "300,600,400,50,200,500,50,550,450,150" }), new Sagittarius(10, 3, new MinMax(3f, 5.1f), 755f, 0.5f, new MinMax(0f, 90f), 1.45f, 495f, 2.5f, new MinMax(5f, 8f)), new Taurus(new MinMax(1.7f, 3.9f), 4f), new Gemini(new MinMax(1.5f, 3f), 1f, 5f, 0.3f, 300f), new UFO(9999f, 325f, 140f, 340f, 0.65f, 1.6f, new string[6] { "A,B,A,A,B,B,A", "B,A,B,B,A,A,B", "A,A,B,A,A,B,B", "B,B,A,B,A,A,A", "A,B,A,B,A,A,B", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 8f, 2f, false), new Gear(2, -255f, 1100f)));
				list.Add(new State(0.39f, new Pattern[1][] { new Pattern[0] }, States.Moon, new Move(5.1f, new MinMax(1.3f, 2.8f)), new DashSummon(new string[1] { "D0.5,1" }, 1f, 2700f, 0.1f, 780f, 3f), new Enemy(true, 10, 805f, 0.4f, new MinMax(10f, 80f), 3.2f, new string[1] { "600-200,500-300,600-400,100-550,150-450,250-50" }, new string[1] { "A,A,A,B" }, new MinMax(0f, 90f), 5, 575f, 525f, new MinMax(1f, 2f)), new Tornado(600f, 1.05f, 1f, 2.6f), new Shoot(450f, 1800f, 1450f, new MinMax(1.2f, 1.5f)), new Morph(0.4f, 2.3f), new Stars(new MinMax(350f, 800f), 5f, 5f, 0.9f, new string[2] { "A,B,C,A,C,A,A,B,C,C,A,B,C,B,A,A,P", "B,C,A,A,C,B,A,B,B,C,A,B,C,C,B,A,P" }, new string[2] { "100,300,400,500,600,200,350,550,50,600", "300,600,400,50,200,500,50,550,450,150" }), new Sagittarius(10, 3, new MinMax(3f, 5.1f), 755f, 0.5f, new MinMax(0f, 90f), 1.45f, 495f, 2.5f, new MinMax(5f, 8f)), new Taurus(new MinMax(1.7f, 3.9f), 4f), new Gemini(new MinMax(1.5f, 3f), 1f, 5f, 0.3f, 300f), new UFO(9999f, 325f, 140f, 340f, 0.65f, 1.6f, new string[6] { "A,B,A,A,B,B,A", "B,A,B,B,A,A,B", "A,A,B,A,A,B,B", "B,B,A,B,A,A,A", "A,B,A,B,A,A,B", "B,A,A,B,A,A,B" }, 0.3f, 0.5f, 1.5f, 8f, 2f, false), new Gear(2, -255f, 1100f)));
				break;
			}
			return new FlyingBlimp(hp, goalTimes, list.ToArray());
		}
	}

	public class FlyingGenie : AbstractLevelProperties<FlyingGenie.State, FlyingGenie.Pattern, FlyingGenie.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected FlyingGenie properties { get; private set; }

			public virtual void LevelInit(FlyingGenie properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			Giant = 2,
			Marionette = 3,
			Disappear = 4
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Pyramids pyramids;

			public readonly GemStone gemStone;

			public readonly Swords swords;

			public readonly Gems gems;

			public readonly Sphinx sphinx;

			public readonly Coffin coffin;

			public readonly Obelisk obelisk;

			public readonly Scan scan;

			public readonly Bomb bomb;

			public readonly Main main;

			public readonly Skull skull;

			public readonly Bullets bullets;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Pyramids pyramids, GemStone gemStone, Swords swords, Gems gems, Sphinx sphinx, Coffin coffin, Obelisk obelisk, Scan scan, Bomb bomb, Main main, Skull skull, Bullets bullets)
				: base(healthTrigger, patterns, stateName)
			{
				this.pyramids = pyramids;
				this.gemStone = gemStone;
				this.swords = swords;
				this.gems = gems;
				this.sphinx = sphinx;
				this.coffin = coffin;
				this.obelisk = obelisk;
				this.scan = scan;
				this.bomb = bomb;
				this.main = main;
				this.skull = skull;
				this.bullets = bullets;
			}
		}

		public class Pyramids : AbstractLevelPropertyGroup
		{
			public readonly float speedRotation;

			public readonly float warningDuration;

			public readonly float beamDuration;

			public readonly string[] attackDelayString;

			public readonly string[] pyramidAttackString;

			public readonly float pyramidLoopSize;

			public Pyramids(float speedRotation, float warningDuration, float beamDuration, string[] attackDelayString, string[] pyramidAttackString, float pyramidLoopSize)
			{
				this.speedRotation = speedRotation;
				this.warningDuration = warningDuration;
				this.beamDuration = beamDuration;
				this.attackDelayString = attackDelayString;
				this.pyramidAttackString = pyramidAttackString;
				this.pyramidLoopSize = pyramidLoopSize;
			}
		}

		public class GemStone : AbstractLevelPropertyGroup
		{
			public readonly float bulletSpeed;

			public readonly float warningDuration;

			public readonly string[] attackDelayString;

			public readonly int ringAmount;

			public readonly string pinkString;

			public GemStone(float bulletSpeed, float warningDuration, string[] attackDelayString, int ringAmount, string pinkString)
			{
				this.bulletSpeed = bulletSpeed;
				this.warningDuration = warningDuration;
				this.attackDelayString = attackDelayString;
				this.ringAmount = ringAmount;
				this.pinkString = pinkString;
			}
		}

		public class Swords : AbstractLevelPropertyGroup
		{
			public readonly float swordSpeed;

			public readonly float appearDelay;

			public readonly float spawnDelay;

			public readonly float attackDelay;

			public readonly string[] patternPositionStrings;

			public readonly float repeatDelay;

			public readonly float hesitate;

			public readonly string swordPinkString;

			public Swords(float swordSpeed, float appearDelay, float spawnDelay, float attackDelay, string[] patternPositionStrings, float repeatDelay, float hesitate, string swordPinkString)
			{
				this.swordSpeed = swordSpeed;
				this.appearDelay = appearDelay;
				this.spawnDelay = spawnDelay;
				this.attackDelay = attackDelay;
				this.patternPositionStrings = patternPositionStrings;
				this.repeatDelay = repeatDelay;
				this.hesitate = hesitate;
				this.swordPinkString = swordPinkString;
			}
		}

		public class Gems : AbstractLevelPropertyGroup
		{
			public readonly float gemSmallSpeed;

			public readonly float gemBigSpeed;

			public readonly MinMax gemSmallDelayRange;

			public readonly MinMax gemBigDelayRange;

			public readonly string[] gemSmallAimOffset;

			public readonly string[] gemBigAimOffset;

			public readonly float gemSmallAttackDuration;

			public readonly float gemBigAttackDuration;

			public readonly float hesitate;

			public readonly float repeatDelay;

			public readonly string gemPinkString;

			public Gems(float gemSmallSpeed, float gemBigSpeed, MinMax gemSmallDelayRange, MinMax gemBigDelayRange, string[] gemSmallAimOffset, string[] gemBigAimOffset, float gemSmallAttackDuration, float gemBigAttackDuration, float hesitate, float repeatDelay, string gemPinkString)
			{
				this.gemSmallSpeed = gemSmallSpeed;
				this.gemBigSpeed = gemBigSpeed;
				this.gemSmallDelayRange = gemSmallDelayRange;
				this.gemBigDelayRange = gemBigDelayRange;
				this.gemSmallAimOffset = gemSmallAimOffset;
				this.gemBigAimOffset = gemBigAimOffset;
				this.gemSmallAttackDuration = gemSmallAttackDuration;
				this.gemBigAttackDuration = gemBigAttackDuration;
				this.hesitate = hesitate;
				this.repeatDelay = repeatDelay;
				this.gemPinkString = gemPinkString;
			}
		}

		public class Sphinx : AbstractLevelPropertyGroup
		{
			public readonly float sphinxSpeed;

			public readonly float sphinxSplitSpeed;

			public readonly string[] sphinxCount;

			public readonly float splitDelay;

			public readonly float miniSpawnDelay;

			public readonly float sphinxMainDelay;

			public readonly string[] sphinxAimX;

			public readonly string[] sphinxAimY;

			public readonly float sphinxSpawnNum;

			public readonly float miniHP;

			public readonly MinMax miniHomingDurationRange;

			public readonly float hesitate;

			public readonly bool dieOnCollisionPlayer;

			public readonly float repeatDelay;

			public readonly float homingSpeed;

			public readonly float homingRotation;

			public readonly string scarabPinkString;

			public Sphinx(float sphinxSpeed, float sphinxSplitSpeed, string[] sphinxCount, float splitDelay, float miniSpawnDelay, float sphinxMainDelay, string[] sphinxAimX, string[] sphinxAimY, float sphinxSpawnNum, float miniHP, MinMax miniHomingDurationRange, float hesitate, bool dieOnCollisionPlayer, float repeatDelay, float homingSpeed, float homingRotation, string scarabPinkString)
			{
				this.sphinxSpeed = sphinxSpeed;
				this.sphinxSplitSpeed = sphinxSplitSpeed;
				this.sphinxCount = sphinxCount;
				this.splitDelay = splitDelay;
				this.miniSpawnDelay = miniSpawnDelay;
				this.sphinxMainDelay = sphinxMainDelay;
				this.sphinxAimX = sphinxAimX;
				this.sphinxAimY = sphinxAimY;
				this.sphinxSpawnNum = sphinxSpawnNum;
				this.miniHP = miniHP;
				this.miniHomingDurationRange = miniHomingDurationRange;
				this.hesitate = hesitate;
				this.dieOnCollisionPlayer = dieOnCollisionPlayer;
				this.repeatDelay = repeatDelay;
				this.homingSpeed = homingSpeed;
				this.homingRotation = homingRotation;
				this.scarabPinkString = scarabPinkString;
			}
		}

		public class Coffin : AbstractLevelPropertyGroup
		{
			public readonly float heartMovement;

			public readonly MinMax heartShotDelayRange;

			public readonly float attackDuration;

			public readonly float heartShotXSpeed;

			public readonly float heartShotYSpeed;

			public readonly float heartLoopYSize;

			public readonly float hesitate;

			public readonly float mummyGenieDelay;

			public readonly float mummyGenieHP;

			public readonly string[] mummyAppearString;

			public readonly string[] mummyGenieDirection;

			public readonly string[] mummyTypeString;

			public readonly float mummyASpeed;

			public readonly float mummyBSpeed;

			public readonly float mummyCSpeed;

			public readonly bool mummyASinWave;

			public readonly bool mummyCSlowdown;

			public Coffin(float heartMovement, MinMax heartShotDelayRange, float attackDuration, float heartShotXSpeed, float heartShotYSpeed, float heartLoopYSize, float hesitate, float mummyGenieDelay, float mummyGenieHP, string[] mummyAppearString, string[] mummyGenieDirection, string[] mummyTypeString, float mummyASpeed, float mummyBSpeed, float mummyCSpeed, bool mummyASinWave, bool mummyCSlowdown)
			{
				this.heartMovement = heartMovement;
				this.heartShotDelayRange = heartShotDelayRange;
				this.attackDuration = attackDuration;
				this.heartShotXSpeed = heartShotXSpeed;
				this.heartShotYSpeed = heartShotYSpeed;
				this.heartLoopYSize = heartLoopYSize;
				this.hesitate = hesitate;
				this.mummyGenieDelay = mummyGenieDelay;
				this.mummyGenieHP = mummyGenieHP;
				this.mummyAppearString = mummyAppearString;
				this.mummyGenieDirection = mummyGenieDirection;
				this.mummyTypeString = mummyTypeString;
				this.mummyASpeed = mummyASpeed;
				this.mummyBSpeed = mummyBSpeed;
				this.mummyCSpeed = mummyCSpeed;
				this.mummyASinWave = mummyASinWave;
				this.mummyCSlowdown = mummyCSlowdown;
			}
		}

		public class Obelisk : AbstractLevelPropertyGroup
		{
			public readonly float obeliskMovementSpeed;

			public readonly int obeliskCount;

			public readonly float obeliskAppearDelay;

			public readonly string[] obeliskGeniePos;

			public readonly float obeliskGenieHP;

			public readonly float obeliskShootDelay;

			public readonly float obeliskShootSpeed;

			public readonly string[] obeliskShotDirection;

			public readonly string[] obeliskPinkString;

			public readonly float bouncerSpeed;

			public readonly string[] bouncerPinkString;

			public readonly string[] bouncerAngleString;

			public readonly bool bounceShotOn;

			public readonly bool normalShotOn;

			public readonly float hesitate;

			public Obelisk(float obeliskMovementSpeed, int obeliskCount, float obeliskAppearDelay, string[] obeliskGeniePos, float obeliskGenieHP, float obeliskShootDelay, float obeliskShootSpeed, string[] obeliskShotDirection, string[] obeliskPinkString, float bouncerSpeed, string[] bouncerPinkString, string[] bouncerAngleString, bool bounceShotOn, bool normalShotOn, float hesitate)
			{
				this.obeliskMovementSpeed = obeliskMovementSpeed;
				this.obeliskCount = obeliskCount;
				this.obeliskAppearDelay = obeliskAppearDelay;
				this.obeliskGeniePos = obeliskGeniePos;
				this.obeliskGenieHP = obeliskGenieHP;
				this.obeliskShootDelay = obeliskShootDelay;
				this.obeliskShootSpeed = obeliskShootSpeed;
				this.obeliskShotDirection = obeliskShotDirection;
				this.obeliskPinkString = obeliskPinkString;
				this.bouncerSpeed = bouncerSpeed;
				this.bouncerPinkString = bouncerPinkString;
				this.bouncerAngleString = bouncerAngleString;
				this.bounceShotOn = bounceShotOn;
				this.normalShotOn = normalShotOn;
				this.hesitate = hesitate;
			}
		}

		public class Scan : AbstractLevelPropertyGroup
		{
			public readonly float scanDuration;

			public readonly float bulletSpeedMin;

			public readonly float bulletSpeedMax;

			public readonly string[] appearCoordinates;

			public readonly float bulletAcceleration;

			public readonly string[] bulletAppearDelay;

			public readonly string bulletPinkString;

			public readonly float bulletWarningDuration;

			public readonly bool usingBulletATK;

			public Scan(float scanDuration, float bulletSpeedMin, float bulletSpeedMax, string[] appearCoordinates, float bulletAcceleration, string[] bulletAppearDelay, string bulletPinkString, float bulletWarningDuration, bool usingBulletATK)
			{
				this.scanDuration = scanDuration;
				this.bulletSpeedMin = bulletSpeedMin;
				this.bulletSpeedMax = bulletSpeedMax;
				this.appearCoordinates = appearCoordinates;
				this.bulletAcceleration = bulletAcceleration;
				this.bulletAppearDelay = bulletAppearDelay;
				this.bulletPinkString = bulletPinkString;
				this.bulletWarningDuration = bulletWarningDuration;
				this.usingBulletATK = usingBulletATK;
			}
		}

		public class Bomb : AbstractLevelPropertyGroup
		{
			public readonly float bombSpeed;

			public readonly float bombDelay;

			public readonly float bombRegularSize;

			public readonly float bombPlusSize;

			public readonly float bombDiagonalSize;

			public readonly string[] bombPlacementString;

			public readonly float hesitate;

			public Bomb(float bombSpeed, float bombDelay, float bombRegularSize, float bombPlusSize, float bombDiagonalSize, string[] bombPlacementString, float hesitate)
			{
				this.bombSpeed = bombSpeed;
				this.bombDelay = bombDelay;
				this.bombRegularSize = bombRegularSize;
				this.bombPlusSize = bombPlusSize;
				this.bombDiagonalSize = bombDiagonalSize;
				this.bombPlacementString = bombPlacementString;
				this.hesitate = hesitate;
			}
		}

		public class Main : AbstractLevelPropertyGroup
		{
			public readonly float introHesitate;

			public Main(float introHesitate)
			{
				this.introHesitate = introHesitate;
			}
		}

		public class Skull : AbstractLevelPropertyGroup
		{
			public readonly MinMax skullDelayRange;

			public readonly float skullSpeed;

			public readonly int skullCount;

			public Skull(MinMax skullDelayRange, float skullSpeed, int skullCount)
			{
				this.skullDelayRange = skullDelayRange;
				this.skullSpeed = skullSpeed;
				this.skullCount = skullCount;
			}
		}

		public class Bullets : AbstractLevelPropertyGroup
		{
			public readonly float shotSpeed;

			public readonly string[] shotCount;

			public readonly float shotDelay;

			public readonly float spawnerSpeed;

			public readonly float spawnerRotateSpeed;

			public readonly int spawnerCount;

			public readonly float spawnerShotDelay;

			public readonly float spawnerDistance;

			public readonly MinMax spawnerMoveCountRange;

			public readonly float spawnerHesitate;

			public readonly int spawnerShotCount;

			public readonly float spawnerMoveDelay;

			public readonly float childSpeed;

			public readonly MinMax hesitateRange;

			public readonly string pinkString;

			public readonly float marionetteMoveSpeed;

			public readonly float marionetteReturnSpeed;

			public Bullets(float shotSpeed, string[] shotCount, float shotDelay, float spawnerSpeed, float spawnerRotateSpeed, int spawnerCount, float spawnerShotDelay, float spawnerDistance, MinMax spawnerMoveCountRange, float spawnerHesitate, int spawnerShotCount, float spawnerMoveDelay, float childSpeed, MinMax hesitateRange, string pinkString, float marionetteMoveSpeed, float marionetteReturnSpeed)
			{
				this.shotSpeed = shotSpeed;
				this.shotCount = shotCount;
				this.shotDelay = shotDelay;
				this.spawnerSpeed = spawnerSpeed;
				this.spawnerRotateSpeed = spawnerRotateSpeed;
				this.spawnerCount = spawnerCount;
				this.spawnerShotDelay = spawnerShotDelay;
				this.spawnerDistance = spawnerDistance;
				this.spawnerMoveCountRange = spawnerMoveCountRange;
				this.spawnerHesitate = spawnerHesitate;
				this.spawnerShotCount = spawnerShotCount;
				this.spawnerMoveDelay = spawnerMoveDelay;
				this.childSpeed = childSpeed;
				this.hesitateRange = hesitateRange;
				this.pinkString = pinkString;
				this.marionetteMoveSpeed = marionetteMoveSpeed;
				this.marionetteReturnSpeed = marionetteReturnSpeed;
			}
		}

		public FlyingGenie(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 2200f;
				timeline.events.Add(new Level.Timeline.Event("Disappear", 0.8f));
				timeline.events.Add(new Level.Timeline.Event("Marionette", 0.4f));
				break;
			case Level.Mode.Normal:
				timeline.health = 2600f;
				timeline.events.Add(new Level.Timeline.Event("Disappear", 0.85f));
				timeline.events.Add(new Level.Timeline.Event("Marionette", 0.6f));
				timeline.events.Add(new Level.Timeline.Event("Giant", 0.25f));
				break;
			case Level.Mode.Hard:
				timeline.health = 3000f;
				timeline.events.Add(new Level.Timeline.Event("Disappear", 0.85f));
				timeline.events.Add(new Level.Timeline.Event("Marionette", 0.6f));
				timeline.events.Add(new Level.Timeline.Event("Giant", 0.25f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern FlyingGenie.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static FlyingGenie GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 2200;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Pyramids(0.9f, 1f, 1f, new string[1] { "3,4,2.5,4.3,3,2,3.4" }, new string[1] { "2,3,1,2,1,3,1,2,3" }, 320f), new GemStone(350f, 1f, new string[1] { "1,3,2,2,1,1.5,3,1,3,1.5" }, 1, string.Empty), new Swords(550f, 1f, 0.9f, 2.3f, new string[2] { "650-650,350-650,50-650,50-500,50-300,50-50,350-50,650-50", "650-50,350-50,50-50,50-300,50-500,50-650,350-650,650-650" }, 3f, 0f, "P,R,R,R,R,P,R,R,R,R,R"), new Gems(550f, 350f, new MinMax(0.25f, 0.4f), new MinMax(2.5f, 3.5f), new string[2] { "0,200,-150,400,-200,0,145,60,-400,0,350,-275,45,-60,0,375,-400,15,450,105,-80,0,-450,80,250,-225,-68", "50,250,-300,0,300,0,-350,-50,15,100,430,0,-250,250,0,-400,175,0,-45,0,300,-75,15,-50,-200,30,350" }, new string[2] { "0,50,25,-75,40,0,50,-50,100", "50,0,-50,200,150,-100,0,-150" }, 8f, 7f, 0f, 3f, "P,R,R,R,R,R,R,R,R,R,R,R"), new Sphinx(400f, 300f, new string[1] { "2,3,3,2,3" }, 1f, 0.75f, 3.6f, new string[2] { "0,255,145,160,360,200", "400,50,300,188,340,110" }, new string[4] { "100,600,350,477", "75,550,255,435", "400,300,500,125", "325,525,115,425" }, 4f, 20f, new MinMax(0.7f, 1.4f), 0f, false, 4.5f, 200f, 2.4f, "P,R,R,R,R,R,P,R,R,R,R,R,R,R"), new Coffin(100f, new MinMax(1.5f, 2.5f), 8.3f, 300f, 15f, 30f, 1f, 1.75f, 16f, new string[2] { "100,250,-100,200,-200,50,250,-150,0,250,50,-150,250,-250", "0,-250,100,250,50,-150,0,250,-150,250,50,200,0,-100,250,-250,50,-200,100,250,0,-150" }, new string[2] { "0,3,0,5,3,-4,0,-4,1,0,0,-5,2,-2,0,-4,3,-3", "1,3,-3,0,2,0,-5,4,-1,2,-2,-5,0,1,2,-1,3,-4,2,-3,0,1,-1" }, new string[3] { "A,B,C,A,C,B,C,A,B,C,B", "A,B,C,B,A,B,C,A,B,C,A,C", "A,B,A,B,C,A,B,C,B,A,C" }, 350f, 550f, 450f, true, false), new Obelisk(325f, 8, 2.25f, new string[3] { "1,4,2,5,1-4,5,1,3,2-5", "3,1,2-5,4,2,3,1-4,2,5", "4,1,2,1-4,3,5,1,3,2-5" }, 20f, 3f, 300f, new string[0], new string[1] { "R,P,R,R,P" }, 475f, new string[1] { "R,P,R,R,P" }, new string[2] { "45,300,40,290,50,310", "295,40,310,50,305,45" }, false, false, 5f), new Scan(1f, 0f, 0f, new string[1] { "9999-9999" }, 0f, new string[1] { "1" }, "R,R,R,P,R,R,R,P,R,R,R,R,P", 1f, true), new Bomb(0f, 0f, 0f, 0f, 0f, new string[0], 0f), new Main(0f), new Skull(new MinMax(3f, 5.4f), 575f, 0), new Bullets(555f, new string[7] { "3", "4", "3", "5", "2", "3", "5" }, 0.8f, 300f, 45f, 4, 0.6f, 275f, new MinMax(9999f, 9999f), 2f, 0, 1f, 200f, new MinMax(2.75f, 2.75f), "R,R,P", 125f, 100f)));
				list.Add(new State(0.8f, new Pattern[1][] { new Pattern[0] }, States.Disappear, new Pyramids(0.9f, 1f, 1f, new string[1] { "3,4,2.5,4.3,3,2,3.4" }, new string[1] { "2,3,1,2,1,3,1,2,3" }, 320f), new GemStone(350f, 1f, new string[1] { "1,3,2,2,1,1.5,3,1,3,1.5" }, 1, string.Empty), new Swords(550f, 1f, 0.9f, 2.3f, new string[2] { "650-650,350-650,50-650,50-500,50-300,50-50,350-50,650-50", "650-50,350-50,50-50,50-300,50-500,50-650,350-650,650-650" }, 3f, 0f, "P,R,R,R,R,P,R,R,R,R,R"), new Gems(550f, 350f, new MinMax(0.25f, 0.4f), new MinMax(2.5f, 3.5f), new string[2] { "0,200,-150,400,-200,0,145,60,-400,0,350,-275,45,-60,0,375,-400,15,450,105,-80,0,-450,80,250,-225,-68", "50,250,-300,0,300,0,-350,-50,15,100,430,0,-250,250,0,-400,175,0,-45,0,300,-75,15,-50,-200,30,350" }, new string[2] { "0,50,25,-75,40,0,50,-50,100", "50,0,-50,200,150,-100,0,-150" }, 8f, 7f, 0f, 3f, "P,R,R,R,R,R,R,R,R,R,R,R"), new Sphinx(400f, 300f, new string[1] { "2,3,3,2,3" }, 1f, 0.75f, 3.6f, new string[2] { "0,255,145,160,360,200", "400,50,300,188,340,110" }, new string[4] { "100,600,350,477", "75,550,255,435", "400,300,500,125", "325,525,115,425" }, 4f, 20f, new MinMax(0.7f, 1.4f), 0f, false, 4.5f, 200f, 2.4f, "P,R,R,R,R,R,P,R,R,R,R,R,R,R"), new Coffin(100f, new MinMax(1.5f, 2.5f), 8.3f, 300f, 15f, 30f, 1f, 1.75f, 16f, new string[2] { "100,250,-100,200,-200,50,250,-150,0,250,50,-150,250,-250", "0,-250,100,250,50,-150,0,250,-150,250,50,200,0,-100,250,-250,50,-200,100,250,0,-150" }, new string[2] { "0,3,0,5,3,-4,0,-4,1,0,0,-5,2,-2,0,-4,3,-3", "1,3,-3,0,2,0,-5,4,-1,2,-2,-5,0,1,2,-1,3,-4,2,-3,0,1,-1" }, new string[3] { "A,B,C,A,C,B,C,A,B,C,B", "A,B,C,B,A,B,C,A,B,C,A,C", "A,B,A,B,C,A,B,C,B,A,C" }, 350f, 550f, 450f, true, false), new Obelisk(325f, 8, 2.25f, new string[3] { "1,4,2,5,1-4,5,1,3,2-5", "3,1,2-5,4,2,3,1-4,2,5", "4,1,2,1-4,3,5,1,3,2-5" }, 20f, 3f, 300f, new string[0], new string[1] { "R,P,R,R,P" }, 475f, new string[1] { "R,P,R,R,P" }, new string[2] { "45,300,40,290,50,310", "295,40,310,50,305,45" }, false, false, 5f), new Scan(1f, 0f, 0f, new string[1] { "9999-9999" }, 0f, new string[1] { "1" }, "R,R,R,P,R,R,R,P,R,R,R,R,P", 1f, true), new Bomb(0f, 0f, 0f, 0f, 0f, new string[0], 0f), new Main(0f), new Skull(new MinMax(3f, 5.4f), 575f, 0), new Bullets(555f, new string[7] { "3", "4", "3", "5", "2", "3", "5" }, 0.8f, 300f, 45f, 4, 0.6f, 275f, new MinMax(9999f, 9999f), 2f, 0, 1f, 200f, new MinMax(2.75f, 2.75f), "R,R,P", 125f, 100f)));
				list.Add(new State(0.4f, new Pattern[1][] { new Pattern[0] }, States.Marionette, new Pyramids(0.9f, 1f, 1f, new string[1] { "3,4,2.5,4.3,3,2,3.4" }, new string[1] { "2,3,1,2,1,3,1,2,3" }, 320f), new GemStone(350f, 1f, new string[1] { "1,3,2,2,1,1.5,3,1,3,1.5" }, 1, string.Empty), new Swords(550f, 1f, 0.9f, 2.3f, new string[2] { "650-650,350-650,50-650,50-500,50-300,50-50,350-50,650-50", "650-50,350-50,50-50,50-300,50-500,50-650,350-650,650-650" }, 3f, 0f, "P,R,R,R,R,P,R,R,R,R,R"), new Gems(550f, 350f, new MinMax(0.25f, 0.4f), new MinMax(2.5f, 3.5f), new string[2] { "0,200,-150,400,-200,0,145,60,-400,0,350,-275,45,-60,0,375,-400,15,450,105,-80,0,-450,80,250,-225,-68", "50,250,-300,0,300,0,-350,-50,15,100,430,0,-250,250,0,-400,175,0,-45,0,300,-75,15,-50,-200,30,350" }, new string[2] { "0,50,25,-75,40,0,50,-50,100", "50,0,-50,200,150,-100,0,-150" }, 8f, 7f, 0f, 3f, "P,R,R,R,R,R,R,R,R,R,R,R"), new Sphinx(400f, 300f, new string[1] { "2,3,3,2,3" }, 1f, 0.75f, 3.6f, new string[2] { "0,255,145,160,360,200", "400,50,300,188,340,110" }, new string[4] { "100,600,350,477", "75,550,255,435", "400,300,500,125", "325,525,115,425" }, 4f, 20f, new MinMax(0.7f, 1.4f), 0f, false, 4.5f, 200f, 2.4f, "P,R,R,R,R,R,P,R,R,R,R,R,R,R"), new Coffin(100f, new MinMax(1.5f, 2.5f), 8.3f, 300f, 15f, 30f, 1f, 1.75f, 16f, new string[2] { "100,250,-100,200,-200,50,250,-150,0,250,50,-150,250,-250", "0,-250,100,250,50,-150,0,250,-150,250,50,200,0,-100,250,-250,50,-200,100,250,0,-150" }, new string[2] { "0,3,0,5,3,-4,0,-4,1,0,0,-5,2,-2,0,-4,3,-3", "1,3,-3,0,2,0,-5,4,-1,2,-2,-5,0,1,2,-1,3,-4,2,-3,0,1,-1" }, new string[3] { "A,B,C,A,C,B,C,A,B,C,B", "A,B,C,B,A,B,C,A,B,C,A,C", "A,B,A,B,C,A,B,C,B,A,C" }, 350f, 550f, 450f, true, false), new Obelisk(325f, 8, 2.25f, new string[3] { "1,4,2,5,1-4,5,1,3,2-5", "3,1,2-5,4,2,3,1-4,2,5", "4,1,2,1-4,3,5,1,3,2-5" }, 20f, 3f, 300f, new string[0], new string[1] { "R,P,R,R,P" }, 475f, new string[1] { "R,P,R,R,P" }, new string[2] { "45,300,40,290,50,310", "295,40,310,50,305,45" }, false, false, 5f), new Scan(1f, 0f, 0f, new string[1] { "9999-9999" }, 0f, new string[1] { "1" }, "R,R,R,P,R,R,R,P,R,R,R,R,P", 1f, true), new Bomb(0f, 0f, 0f, 0f, 0f, new string[0], 0f), new Main(0f), new Skull(new MinMax(3f, 5.4f), 575f, 0), new Bullets(555f, new string[7] { "3", "4", "3", "5", "2", "3", "5" }, 0.8f, 300f, 45f, 4, 0.6f, 275f, new MinMax(9999f, 9999f), 2f, 0, 1f, 200f, new MinMax(2.75f, 2.75f), "R,R,P", 125f, 100f)));
				break;
			case Level.Mode.Normal:
				hp = 2600;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Pyramids(0.9f, 1f, 0.85f, new string[2] { "3,4,2.5,4.3,3,2,3.4", "3.2,2.6,4,2,3,4.5,2.8" }, new string[2] { "2,3,1,2,1,3,1,2,3", "1,2,3,2,1,3,2,3,1,3" }, 320f), new GemStone(350f, 1f, new string[1] { "5.5,4.8,4.6,5.2,4.9,4,5.5,4.7,5.2,5.8,4.4" }, 1, "R"), new Swords(600f, 1f, 1f, 2f, new string[8] { "50-50,250-50,450-50,650-50", "50-650,50-500,50-300,50-50", "50-650,250-650,450-650,650-650", "50-50,50-300,50-500,50-650", "650-50,450-50,250-50,50-50", "50-650,50-500,50-300,50-50", "650-650,450-650,250-650,50-650", "50-50,50-300,50-500,50-650" }, 2.4f, 0f, "P,R,R,R,R,P,R,R,R,R,R"), new Gems(550f, 350f, new MinMax(0.17f, 0.3f), new MinMax(1.8f, 2.7f), new string[3] { "50,150,-100,0,300,0,-300,-50,15,100,400,0,-200,250,0,-400,175,0,-45,0,300,-75,15,50,-200,30,350", "0,200,-100,400,-100,0,45,100,-400,0,250,-175,45,-60,0,225,-100,15,450,165,-40,0,-450,80,150,-225,-68", "0,200,-150,25,300,45,400,0,100,-350,-150,25,200,-200,0,400,-50,-400,0,50,-100,15,300,-300,0,-100,0,-400,25,-100,-250,0,50,-400,150,0,350" }, new string[2] { "0,100,25,125,-40,0,50,-50,150", "50,0,-50,200,150,-100,0,-150" }, 8f, 7f, 0f, 3f, "P,R,R,R,R,R,R,R,R,R,R,R,R"), new Sphinx(400f, 330f, new string[1] { "2,3,3,2,4" }, 1f, 0.65f, 3.3f, new string[2] { "0,255,145,160,360,200", "400,50,300,188,340,110" }, new string[4] { "100,600,350,477", "75,550,255,435", "400,300,500,125", "325,525,115,425" }, 4f, 20f, new MinMax(2.5f, 3.4f), 0f, false, 4.3f, 200f, 2.4f, "P,R,R,R,R,R,P,R,R,R,R,R,R,R"), new Coffin(200f, new MinMax(1.3f, 2.3f), 8.3f, 300f, 4.5f, 30f, 1f, 1.25f, 16f, new string[2] { "0,100,250,-100,250,200,-200,250,50,-150,0,250,50,-150,250,-250", "0,-250,100,250,50,-150,0,250,-150,250,50,200,0,-100,250,-250,50,-200,100,250,0,-150" }, new string[2] { "0,3,0,5,3,-4,0,-4,1,0,0,-5,2,-2,0,-4,3,-3", "1,3,-3,0,2,0,-5,4,-1,2,-2,-5,0,1,2,-1,3,-4,2,-3,0,1,-1" }, new string[3] { "A,B,C,A,C,B,C,A,B,C,B", "A,B,C,B,A,B,C,A,B,C,A,C", "A,B,A,B,C,A,B,C,B,A,C" }, 400f, 600f, 500f, true, false), new Obelisk(265f, 6, 2.95f, new string[3] { "1,4,2,5,1-4,5,1,3,2-5", "3,1,2-5,4,2,3,1-4,2,5", "4,1,2,1-4,3,5,1,3,2-5" }, 20f, 3f, 300f, new string[1] { "40,300" }, new string[1] { "R,R,P,R,R,R,P" }, 435f, new string[1] { "R,P,R,R,P" }, new string[2] { "45,300,40,290,50,310", "295,40,310,50,305,45" }, true, false, 5f), new Scan(1f, 0f, 0f, new string[1] { "9999-9999" }, 0f, new string[1] { "1" }, "R,R,R,P,R,R,R,R,P,R,R,R,R,R,P", 1f, true), new Bomb(200f, 1f, 1f, 1.5f, 1.2f, new string[2] { "R:200-400,D:400-550,P:100-300", "R:600-600,R:200-350,R:300-500" }, 5f), new Main(1f), new Skull(new MinMax(1f, 2f), 575f, 1), new Bullets(590f, new string[6] { "3,5", "4,4", "5,3", "4,4", "3,5", "5,3" }, 0.7f, 275f, 44f, 4, 0.75f, 300f, new MinMax(4f, 6f), 1.75f, 3, 0.8f, 200f, new MinMax(2.75f, 2.75f), "R,R,R,P", 125f, 100f)));
				list.Add(new State(0.85f, new Pattern[1][] { new Pattern[0] }, States.Disappear, new Pyramids(0.9f, 1f, 0.85f, new string[2] { "3,4,2.5,4.3,3,2,3.4", "3.2,2.6,4,2,3,4.5,2.8" }, new string[2] { "2,3,1,2,1,3,1,2,3", "1,2,3,2,1,3,2,3,1,3" }, 320f), new GemStone(350f, 1f, new string[1] { "5.5,4.8,4.6,5.2,4.9,4,5.5,4.7,5.2,5.8,4.4" }, 1, "R"), new Swords(600f, 1f, 1f, 2f, new string[8] { "50-50,250-50,450-50,650-50", "50-650,50-500,50-300,50-50", "50-650,250-650,450-650,650-650", "50-50,50-300,50-500,50-650", "650-50,450-50,250-50,50-50", "50-650,50-500,50-300,50-50", "650-650,450-650,250-650,50-650", "50-50,50-300,50-500,50-650" }, 2.4f, 0f, "P,R,R,R,R,P,R,R,R,R,R"), new Gems(550f, 350f, new MinMax(0.17f, 0.3f), new MinMax(1.8f, 2.7f), new string[3] { "50,150,-100,0,300,0,-300,-50,15,100,400,0,-200,250,0,-400,175,0,-45,0,300,-75,15,50,-200,30,350", "0,200,-100,400,-100,0,45,100,-400,0,250,-175,45,-60,0,225,-100,15,450,165,-40,0,-450,80,150,-225,-68", "0,200,-150,25,300,45,400,0,100,-350,-150,25,200,-200,0,400,-50,-400,0,50,-100,15,300,-300,0,-100,0,-400,25,-100,-250,0,50,-400,150,0,350" }, new string[2] { "0,100,25,125,-40,0,50,-50,150", "50,0,-50,200,150,-100,0,-150" }, 8f, 7f, 0f, 3f, "P,R,R,R,R,R,R,R,R,R,R,R,R"), new Sphinx(400f, 330f, new string[1] { "2,3,3,2,4" }, 1f, 0.65f, 3.3f, new string[2] { "0,255,145,160,360,200", "400,50,300,188,340,110" }, new string[4] { "100,600,350,477", "75,550,255,435", "400,300,500,125", "325,525,115,425" }, 4f, 20f, new MinMax(2.5f, 3.4f), 0f, false, 4.3f, 200f, 2.4f, "P,R,R,R,R,R,P,R,R,R,R,R,R,R"), new Coffin(200f, new MinMax(1.3f, 2.3f), 8.3f, 300f, 4.5f, 30f, 1f, 1.25f, 16f, new string[2] { "0,100,250,-100,250,200,-200,250,50,-150,0,250,50,-150,250,-250", "0,-250,100,250,50,-150,0,250,-150,250,50,200,0,-100,250,-250,50,-200,100,250,0,-150" }, new string[2] { "0,3,0,5,3,-4,0,-4,1,0,0,-5,2,-2,0,-4,3,-3", "1,3,-3,0,2,0,-5,4,-1,2,-2,-5,0,1,2,-1,3,-4,2,-3,0,1,-1" }, new string[3] { "A,B,C,A,C,B,C,A,B,C,B", "A,B,C,B,A,B,C,A,B,C,A,C", "A,B,A,B,C,A,B,C,B,A,C" }, 400f, 600f, 500f, true, false), new Obelisk(265f, 6, 2.95f, new string[3] { "1,4,2,5,1-4,5,1,3,2-5", "3,1,2-5,4,2,3,1-4,2,5", "4,1,2,1-4,3,5,1,3,2-5" }, 20f, 3f, 300f, new string[1] { "40,300" }, new string[1] { "R,R,P,R,R,R,P" }, 435f, new string[1] { "R,P,R,R,P" }, new string[2] { "45,300,40,290,50,310", "295,40,310,50,305,45" }, true, false, 5f), new Scan(1f, 0f, 0f, new string[1] { "9999-9999" }, 0f, new string[1] { "1" }, "R,R,R,P,R,R,R,R,P,R,R,R,R,R,P", 1f, true), new Bomb(200f, 1f, 1f, 1.5f, 1.2f, new string[2] { "R:200-400,D:400-550,P:100-300", "R:600-600,R:200-350,R:300-500" }, 5f), new Main(1f), new Skull(new MinMax(1f, 2f), 575f, 1), new Bullets(590f, new string[6] { "3,5", "4,4", "5,3", "4,4", "3,5", "5,3" }, 0.7f, 275f, 44f, 4, 0.75f, 300f, new MinMax(4f, 6f), 1.75f, 3, 0.8f, 200f, new MinMax(2.75f, 2.75f), "R,R,R,P", 125f, 100f)));
				list.Add(new State(0.6f, new Pattern[1][] { new Pattern[0] }, States.Marionette, new Pyramids(0.9f, 1f, 0.85f, new string[2] { "3,4,2.5,4.3,3,2,3.4", "3.2,2.6,4,2,3,4.5,2.8" }, new string[2] { "2,3,1,2,1,3,1,2,3", "1,2,3,2,1,3,2,3,1,3" }, 320f), new GemStone(350f, 1f, new string[1] { "5.5,4.8,4.6,5.2,4.9,4,5.5,4.7,5.2,5.8,4.4" }, 1, "R"), new Swords(600f, 1f, 1f, 2f, new string[8] { "50-50,250-50,450-50,650-50", "50-650,50-500,50-300,50-50", "50-650,250-650,450-650,650-650", "50-50,50-300,50-500,50-650", "650-50,450-50,250-50,50-50", "50-650,50-500,50-300,50-50", "650-650,450-650,250-650,50-650", "50-50,50-300,50-500,50-650" }, 2.4f, 0f, "P,R,R,R,R,P,R,R,R,R,R"), new Gems(550f, 350f, new MinMax(0.17f, 0.3f), new MinMax(1.8f, 2.7f), new string[3] { "50,150,-100,0,300,0,-300,-50,15,100,400,0,-200,250,0,-400,175,0,-45,0,300,-75,15,50,-200,30,350", "0,200,-100,400,-100,0,45,100,-400,0,250,-175,45,-60,0,225,-100,15,450,165,-40,0,-450,80,150,-225,-68", "0,200,-150,25,300,45,400,0,100,-350,-150,25,200,-200,0,400,-50,-400,0,50,-100,15,300,-300,0,-100,0,-400,25,-100,-250,0,50,-400,150,0,350" }, new string[2] { "0,100,25,125,-40,0,50,-50,150", "50,0,-50,200,150,-100,0,-150" }, 8f, 7f, 0f, 3f, "P,R,R,R,R,R,R,R,R,R,R,R,R"), new Sphinx(400f, 330f, new string[1] { "2,3,3,2,4" }, 1f, 0.65f, 3.3f, new string[2] { "0,255,145,160,360,200", "400,50,300,188,340,110" }, new string[4] { "100,600,350,477", "75,550,255,435", "400,300,500,125", "325,525,115,425" }, 4f, 20f, new MinMax(2.5f, 3.4f), 0f, false, 4.3f, 200f, 2.4f, "P,R,R,R,R,R,P,R,R,R,R,R,R,R"), new Coffin(200f, new MinMax(1.3f, 2.3f), 8.3f, 300f, 4.5f, 30f, 1f, 1.25f, 16f, new string[2] { "0,100,250,-100,250,200,-200,250,50,-150,0,250,50,-150,250,-250", "0,-250,100,250,50,-150,0,250,-150,250,50,200,0,-100,250,-250,50,-200,100,250,0,-150" }, new string[2] { "0,3,0,5,3,-4,0,-4,1,0,0,-5,2,-2,0,-4,3,-3", "1,3,-3,0,2,0,-5,4,-1,2,-2,-5,0,1,2,-1,3,-4,2,-3,0,1,-1" }, new string[3] { "A,B,C,A,C,B,C,A,B,C,B", "A,B,C,B,A,B,C,A,B,C,A,C", "A,B,A,B,C,A,B,C,B,A,C" }, 400f, 600f, 500f, true, false), new Obelisk(265f, 6, 2.95f, new string[3] { "1,4,2,5,1-4,5,1,3,2-5", "3,1,2-5,4,2,3,1-4,2,5", "4,1,2,1-4,3,5,1,3,2-5" }, 20f, 3f, 300f, new string[1] { "40,300" }, new string[1] { "R,R,P,R,R,R,P" }, 435f, new string[1] { "R,P,R,R,P" }, new string[2] { "45,300,40,290,50,310", "295,40,310,50,305,45" }, true, false, 5f), new Scan(1f, 0f, 0f, new string[1] { "9999-9999" }, 0f, new string[1] { "1" }, "R,R,R,P,R,R,R,R,P,R,R,R,R,R,P", 1f, true), new Bomb(200f, 1f, 1f, 1.5f, 1.2f, new string[2] { "R:200-400,D:400-550,P:100-300", "R:600-600,R:200-350,R:300-500" }, 5f), new Main(1f), new Skull(new MinMax(1f, 2f), 575f, 1), new Bullets(590f, new string[6] { "3,5", "4,4", "5,3", "4,4", "3,5", "5,3" }, 0.7f, 275f, 44f, 4, 0.75f, 300f, new MinMax(4f, 6f), 1.75f, 3, 0.8f, 200f, new MinMax(2.75f, 2.75f), "R,R,R,P", 125f, 100f)));
				list.Add(new State(0.25f, new Pattern[1][] { new Pattern[0] }, States.Giant, new Pyramids(0.9f, 1f, 0.85f, new string[2] { "3,4,2.5,4.3,3,2,3.4", "3.2,2.6,4,2,3,4.5,2.8" }, new string[2] { "2,3,1,2,1,3,1,2,3", "1,2,3,2,1,3,2,3,1,3" }, 320f), new GemStone(350f, 1f, new string[1] { "5.5,4.8,4.6,5.2,4.9,4,5.5,4.7,5.2,5.8,4.4" }, 1, "R"), new Swords(600f, 1f, 1f, 2f, new string[8] { "50-50,250-50,450-50,650-50", "50-650,50-500,50-300,50-50", "50-650,250-650,450-650,650-650", "50-50,50-300,50-500,50-650", "650-50,450-50,250-50,50-50", "50-650,50-500,50-300,50-50", "650-650,450-650,250-650,50-650", "50-50,50-300,50-500,50-650" }, 2.4f, 0f, "P,R,R,R,R,P,R,R,R,R,R"), new Gems(550f, 350f, new MinMax(0.17f, 0.3f), new MinMax(1.8f, 2.7f), new string[3] { "50,150,-100,0,300,0,-300,-50,15,100,400,0,-200,250,0,-400,175,0,-45,0,300,-75,15,50,-200,30,350", "0,200,-100,400,-100,0,45,100,-400,0,250,-175,45,-60,0,225,-100,15,450,165,-40,0,-450,80,150,-225,-68", "0,200,-150,25,300,45,400,0,100,-350,-150,25,200,-200,0,400,-50,-400,0,50,-100,15,300,-300,0,-100,0,-400,25,-100,-250,0,50,-400,150,0,350" }, new string[2] { "0,100,25,125,-40,0,50,-50,150", "50,0,-50,200,150,-100,0,-150" }, 8f, 7f, 0f, 3f, "P,R,R,R,R,R,R,R,R,R,R,R,R"), new Sphinx(400f, 330f, new string[1] { "2,3,3,2,4" }, 1f, 0.65f, 3.3f, new string[2] { "0,255,145,160,360,200", "400,50,300,188,340,110" }, new string[4] { "100,600,350,477", "75,550,255,435", "400,300,500,125", "325,525,115,425" }, 4f, 20f, new MinMax(2.5f, 3.4f), 0f, false, 4.3f, 200f, 2.4f, "P,R,R,R,R,R,P,R,R,R,R,R,R,R"), new Coffin(200f, new MinMax(1.3f, 2.3f), 8.3f, 300f, 4.5f, 30f, 1f, 1.25f, 16f, new string[2] { "0,100,250,-100,250,200,-200,250,50,-150,0,250,50,-150,250,-250", "0,-250,100,250,50,-150,0,250,-150,250,50,200,0,-100,250,-250,50,-200,100,250,0,-150" }, new string[2] { "0,3,0,5,3,-4,0,-4,1,0,0,-5,2,-2,0,-4,3,-3", "1,3,-3,0,2,0,-5,4,-1,2,-2,-5,0,1,2,-1,3,-4,2,-3,0,1,-1" }, new string[3] { "A,B,C,A,C,B,C,A,B,C,B", "A,B,C,B,A,B,C,A,B,C,A,C", "A,B,A,B,C,A,B,C,B,A,C" }, 400f, 600f, 500f, true, false), new Obelisk(265f, 6, 2.95f, new string[3] { "1,4,2,5,1-4,5,1,3,2-5", "3,1,2-5,4,2,3,1-4,2,5", "4,1,2,1-4,3,5,1,3,2-5" }, 20f, 3f, 300f, new string[1] { "40,300" }, new string[1] { "R,R,P,R,R,R,P" }, 435f, new string[1] { "R,P,R,R,P" }, new string[2] { "45,300,40,290,50,310", "295,40,310,50,305,45" }, true, false, 5f), new Scan(1f, 0f, 0f, new string[1] { "9999-9999" }, 0f, new string[1] { "1" }, "R,R,R,P,R,R,R,R,P,R,R,R,R,R,P", 1f, true), new Bomb(200f, 1f, 1f, 1.5f, 1.2f, new string[2] { "R:200-400,D:400-550,P:100-300", "R:600-600,R:200-350,R:300-500" }, 5f), new Main(1f), new Skull(new MinMax(1f, 2f), 575f, 1), new Bullets(590f, new string[6] { "3,5", "4,4", "5,3", "4,4", "3,5", "5,3" }, 0.7f, 275f, 44f, 4, 0.75f, 300f, new MinMax(4f, 6f), 1.75f, 3, 0.8f, 200f, new MinMax(2.75f, 2.75f), "R,R,R,P", 125f, 100f)));
				break;
			case Level.Mode.Hard:
				hp = 3000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Pyramids(0.9f, 1f, 1.05f, new string[2] { "3,3.4,2.5,4,2.8,2,3.4", "3.2,2.6,3.5,2.3,3,2.7,2" }, new string[2] { "2,3,1,2,1,3,1,2,3", "1,2,3,2,1,3,2,3,1,3" }, 320f), new GemStone(350f, 1f, new string[1] { "2.7,3,2.5,3.5,2.6,4,3.3,2.5,3.1,4.1,2.7,3.6,2.8" }, 1, "R"), new Swords(600f, 1f, 0.9f, 2.4f, new string[2] { "650-50,650-650,350-50,350-650,50-50,50-650,50-300,50-500", "650-650,650-50,350-650-350-50,50-650,50-50,50-500,50-300" }, 2.5f, 0f, "P,R,R,R,R,P,R,R,R,R,R,R"), new Gems(600f, 400f, new MinMax(0.11f, 0.25f), new MinMax(1.5f, 2.5f), new string[3] { "0,200,-150,25,300,45,400,0,100,-350,-150,25,200,-200,0,400,-50,-400,0,50,-100,15,300,-300,0,-100,0,-400,25,-100,-250,0,50,-400,150,0,350", "50,150,-100,0,300,0,-300,-50,15,100,400,0,-200,250,0,-400,175,0,-45,0,300,-75,15,50,-200,30,350", "0,200,-100,400,-100,0,45,100,-400,0,250,-175,45,-60,0,225,-100,15,450,165,-40,0,-450,80,150,-225,-68" }, new string[2] { "0,100,25,125,-40,0,50,-50,150", "50,0,-50,200,150,-100,0,-150" }, 8f, 7f, 0f, 2.5f, "P,R,R,R,R,R,R,R,R,R,R,R,R"), new Sphinx(400f, 300f, new string[1] { "3,3,2,4,3,4" }, 1f, 0.6f, 3.3f, new string[2] { "0,255,145,160,360,200", "400,50,300,188,340,110" }, new string[4] { "100,600,350,477", "75,550,255,435", "400,300,500,125", "325,525,115,425" }, 6f, 20f, new MinMax(2.5f, 3.4f), 0f, false, 4.3f, 200f, 2.4f, "P,R,R,R,R,R,P,R,R,R,R,R,R,R"), new Coffin(200f, new MinMax(0.6f, 1.3f), 8.3f, 300f, 5.5f, 30f, 1f, 0.8f, 16f, new string[2] { "250,0,100,250,-100,200,-200,250,50,-150,0,250,50,-150,250,-250,-50", "0,-250,250,100,200,50,250,-150,0,250,-150,50,200,0,250,-100,-250,50,250,-200,100,250,0,-150" }, new string[2] { "0,3,0,5,3,-4,0,-4,1,0,0,-5,2,-2,0,-4,3,-3", "1,3,-3,0,2,0,-5,4,-1,2,-2,-5,0,1,2,-1,3,-4,2,-3,0,1,-1" }, new string[3] { "A,B,C,A,C,B,C,A,B,C,B", "A,B,C,B,A,B,C,A,B,C,A,C", "A,B,A,B,C,A,B,C,B,A,C" }, 430f, 630f, 530f, true, false), new Obelisk(285f, 8, 2.45f, new string[3] { "1,4,2,5,1-4,5,1,3,2-5", "3,1,2-5,4,2,3,1-4,2,5", "4,1,2,1-4,3,5,1,3,2-5" }, 20f, 3f, 300f, new string[0], new string[0], 485f, new string[1] { "R,P,R,R,P" }, new string[2] { "45,300,40,290,50,310", "295,40,310,50,305,45" }, true, false, 5f), new Scan(1f, 0f, 0f, new string[1] { "9999-9999" }, 0f, new string[1] { "1" }, "R,R,R,R,P,R,R,R,R,R,P", 1f, true), new Bomb(0f, 0f, 0f, 0f, 0f, new string[0], 0f), new Main(0f), new Skull(new MinMax(2.5f, 5.4f), 575f, 2), new Bullets(635f, new string[5] { "5,4,4", "3,4,6", "4,3,6", "4,5,4", "3,6,4" }, 0.55f, 325f, 44f, 4, 0.65f, 335f, new MinMax(4f, 6f), 1.75f, 4, 0.6f, 225f, new MinMax(2.75f, 2.75f), "R,R,R,P", 145f, 100f)));
				list.Add(new State(0.85f, new Pattern[1][] { new Pattern[0] }, States.Disappear, new Pyramids(0.9f, 1f, 1.05f, new string[2] { "3,3.4,2.5,4,2.8,2,3.4", "3.2,2.6,3.5,2.3,3,2.7,2" }, new string[2] { "2,3,1,2,1,3,1,2,3", "1,2,3,2,1,3,2,3,1,3" }, 320f), new GemStone(350f, 1f, new string[1] { "2.7,3,2.5,3.5,2.6,4,3.3,2.5,3.1,4.1,2.7,3.6,2.8" }, 1, "R"), new Swords(600f, 1f, 0.9f, 2.4f, new string[2] { "650-50,650-650,350-50,350-650,50-50,50-650,50-300,50-500", "650-650,650-50,350-650-350-50,50-650,50-50,50-500,50-300" }, 2.5f, 0f, "P,R,R,R,R,P,R,R,R,R,R,R"), new Gems(600f, 400f, new MinMax(0.11f, 0.25f), new MinMax(1.5f, 2.5f), new string[3] { "0,200,-150,25,300,45,400,0,100,-350,-150,25,200,-200,0,400,-50,-400,0,50,-100,15,300,-300,0,-100,0,-400,25,-100,-250,0,50,-400,150,0,350", "50,150,-100,0,300,0,-300,-50,15,100,400,0,-200,250,0,-400,175,0,-45,0,300,-75,15,50,-200,30,350", "0,200,-100,400,-100,0,45,100,-400,0,250,-175,45,-60,0,225,-100,15,450,165,-40,0,-450,80,150,-225,-68" }, new string[2] { "0,100,25,125,-40,0,50,-50,150", "50,0,-50,200,150,-100,0,-150" }, 8f, 7f, 0f, 2.5f, "P,R,R,R,R,R,R,R,R,R,R,R,R"), new Sphinx(400f, 300f, new string[1] { "3,3,2,4,3,4" }, 1f, 0.6f, 3.3f, new string[2] { "0,255,145,160,360,200", "400,50,300,188,340,110" }, new string[4] { "100,600,350,477", "75,550,255,435", "400,300,500,125", "325,525,115,425" }, 6f, 20f, new MinMax(2.5f, 3.4f), 0f, false, 4.3f, 200f, 2.4f, "P,R,R,R,R,R,P,R,R,R,R,R,R,R"), new Coffin(200f, new MinMax(0.6f, 1.3f), 8.3f, 300f, 5.5f, 30f, 1f, 0.8f, 16f, new string[2] { "250,0,100,250,-100,200,-200,250,50,-150,0,250,50,-150,250,-250,-50", "0,-250,250,100,200,50,250,-150,0,250,-150,50,200,0,250,-100,-250,50,250,-200,100,250,0,-150" }, new string[2] { "0,3,0,5,3,-4,0,-4,1,0,0,-5,2,-2,0,-4,3,-3", "1,3,-3,0,2,0,-5,4,-1,2,-2,-5,0,1,2,-1,3,-4,2,-3,0,1,-1" }, new string[3] { "A,B,C,A,C,B,C,A,B,C,B", "A,B,C,B,A,B,C,A,B,C,A,C", "A,B,A,B,C,A,B,C,B,A,C" }, 430f, 630f, 530f, true, false), new Obelisk(285f, 8, 2.45f, new string[3] { "1,4,2,5,1-4,5,1,3,2-5", "3,1,2-5,4,2,3,1-4,2,5", "4,1,2,1-4,3,5,1,3,2-5" }, 20f, 3f, 300f, new string[0], new string[0], 485f, new string[1] { "R,P,R,R,P" }, new string[2] { "45,300,40,290,50,310", "295,40,310,50,305,45" }, true, false, 5f), new Scan(1f, 0f, 0f, new string[1] { "9999-9999" }, 0f, new string[1] { "1" }, "R,R,R,R,P,R,R,R,R,R,P", 1f, true), new Bomb(0f, 0f, 0f, 0f, 0f, new string[0], 0f), new Main(0f), new Skull(new MinMax(2.5f, 5.4f), 575f, 2), new Bullets(635f, new string[5] { "5,4,4", "3,4,6", "4,3,6", "4,5,4", "3,6,4" }, 0.55f, 325f, 44f, 4, 0.65f, 335f, new MinMax(4f, 6f), 1.75f, 4, 0.6f, 225f, new MinMax(2.75f, 2.75f), "R,R,R,P", 145f, 100f)));
				list.Add(new State(0.6f, new Pattern[1][] { new Pattern[0] }, States.Marionette, new Pyramids(0.9f, 1f, 1.05f, new string[2] { "3,3.4,2.5,4,2.8,2,3.4", "3.2,2.6,3.5,2.3,3,2.7,2" }, new string[2] { "2,3,1,2,1,3,1,2,3", "1,2,3,2,1,3,2,3,1,3" }, 320f), new GemStone(350f, 1f, new string[1] { "2.7,3,2.5,3.5,2.6,4,3.3,2.5,3.1,4.1,2.7,3.6,2.8" }, 1, "R"), new Swords(600f, 1f, 0.9f, 2.4f, new string[2] { "650-50,650-650,350-50,350-650,50-50,50-650,50-300,50-500", "650-650,650-50,350-650-350-50,50-650,50-50,50-500,50-300" }, 2.5f, 0f, "P,R,R,R,R,P,R,R,R,R,R,R"), new Gems(600f, 400f, new MinMax(0.11f, 0.25f), new MinMax(1.5f, 2.5f), new string[3] { "0,200,-150,25,300,45,400,0,100,-350,-150,25,200,-200,0,400,-50,-400,0,50,-100,15,300,-300,0,-100,0,-400,25,-100,-250,0,50,-400,150,0,350", "50,150,-100,0,300,0,-300,-50,15,100,400,0,-200,250,0,-400,175,0,-45,0,300,-75,15,50,-200,30,350", "0,200,-100,400,-100,0,45,100,-400,0,250,-175,45,-60,0,225,-100,15,450,165,-40,0,-450,80,150,-225,-68" }, new string[2] { "0,100,25,125,-40,0,50,-50,150", "50,0,-50,200,150,-100,0,-150" }, 8f, 7f, 0f, 2.5f, "P,R,R,R,R,R,R,R,R,R,R,R,R"), new Sphinx(400f, 300f, new string[1] { "3,3,2,4,3,4" }, 1f, 0.6f, 3.3f, new string[2] { "0,255,145,160,360,200", "400,50,300,188,340,110" }, new string[4] { "100,600,350,477", "75,550,255,435", "400,300,500,125", "325,525,115,425" }, 6f, 20f, new MinMax(2.5f, 3.4f), 0f, false, 4.3f, 200f, 2.4f, "P,R,R,R,R,R,P,R,R,R,R,R,R,R"), new Coffin(200f, new MinMax(0.6f, 1.3f), 8.3f, 300f, 5.5f, 30f, 1f, 0.8f, 16f, new string[2] { "250,0,100,250,-100,200,-200,250,50,-150,0,250,50,-150,250,-250,-50", "0,-250,250,100,200,50,250,-150,0,250,-150,50,200,0,250,-100,-250,50,250,-200,100,250,0,-150" }, new string[2] { "0,3,0,5,3,-4,0,-4,1,0,0,-5,2,-2,0,-4,3,-3", "1,3,-3,0,2,0,-5,4,-1,2,-2,-5,0,1,2,-1,3,-4,2,-3,0,1,-1" }, new string[3] { "A,B,C,A,C,B,C,A,B,C,B", "A,B,C,B,A,B,C,A,B,C,A,C", "A,B,A,B,C,A,B,C,B,A,C" }, 430f, 630f, 530f, true, false), new Obelisk(285f, 8, 2.45f, new string[3] { "1,4,2,5,1-4,5,1,3,2-5", "3,1,2-5,4,2,3,1-4,2,5", "4,1,2,1-4,3,5,1,3,2-5" }, 20f, 3f, 300f, new string[0], new string[0], 485f, new string[1] { "R,P,R,R,P" }, new string[2] { "45,300,40,290,50,310", "295,40,310,50,305,45" }, true, false, 5f), new Scan(1f, 0f, 0f, new string[1] { "9999-9999" }, 0f, new string[1] { "1" }, "R,R,R,R,P,R,R,R,R,R,P", 1f, true), new Bomb(0f, 0f, 0f, 0f, 0f, new string[0], 0f), new Main(0f), new Skull(new MinMax(2.5f, 5.4f), 575f, 2), new Bullets(635f, new string[5] { "5,4,4", "3,4,6", "4,3,6", "4,5,4", "3,6,4" }, 0.55f, 325f, 44f, 4, 0.65f, 335f, new MinMax(4f, 6f), 1.75f, 4, 0.6f, 225f, new MinMax(2.75f, 2.75f), "R,R,R,P", 145f, 100f)));
				list.Add(new State(0.25f, new Pattern[1][] { new Pattern[0] }, States.Giant, new Pyramids(0.9f, 1f, 1.05f, new string[2] { "3,3.4,2.5,4,2.8,2,3.4", "3.2,2.6,3.5,2.3,3,2.7,2" }, new string[2] { "2,3,1,2,1,3,1,2,3", "1,2,3,2,1,3,2,3,1,3" }, 320f), new GemStone(350f, 1f, new string[1] { "2.7,3,2.5,3.5,2.6,4,3.3,2.5,3.1,4.1,2.7,3.6,2.8" }, 1, "R"), new Swords(600f, 1f, 0.9f, 2.4f, new string[2] { "650-50,650-650,350-50,350-650,50-50,50-650,50-300,50-500", "650-650,650-50,350-650-350-50,50-650,50-50,50-500,50-300" }, 2.5f, 0f, "P,R,R,R,R,P,R,R,R,R,R,R"), new Gems(600f, 400f, new MinMax(0.11f, 0.25f), new MinMax(1.5f, 2.5f), new string[3] { "0,200,-150,25,300,45,400,0,100,-350,-150,25,200,-200,0,400,-50,-400,0,50,-100,15,300,-300,0,-100,0,-400,25,-100,-250,0,50,-400,150,0,350", "50,150,-100,0,300,0,-300,-50,15,100,400,0,-200,250,0,-400,175,0,-45,0,300,-75,15,50,-200,30,350", "0,200,-100,400,-100,0,45,100,-400,0,250,-175,45,-60,0,225,-100,15,450,165,-40,0,-450,80,150,-225,-68" }, new string[2] { "0,100,25,125,-40,0,50,-50,150", "50,0,-50,200,150,-100,0,-150" }, 8f, 7f, 0f, 2.5f, "P,R,R,R,R,R,R,R,R,R,R,R,R"), new Sphinx(400f, 300f, new string[1] { "3,3,2,4,3,4" }, 1f, 0.6f, 3.3f, new string[2] { "0,255,145,160,360,200", "400,50,300,188,340,110" }, new string[4] { "100,600,350,477", "75,550,255,435", "400,300,500,125", "325,525,115,425" }, 6f, 20f, new MinMax(2.5f, 3.4f), 0f, false, 4.3f, 200f, 2.4f, "P,R,R,R,R,R,P,R,R,R,R,R,R,R"), new Coffin(200f, new MinMax(0.6f, 1.3f), 8.3f, 300f, 5.5f, 30f, 1f, 0.8f, 16f, new string[2] { "250,0,100,250,-100,200,-200,250,50,-150,0,250,50,-150,250,-250,-50", "0,-250,250,100,200,50,250,-150,0,250,-150,50,200,0,250,-100,-250,50,250,-200,100,250,0,-150" }, new string[2] { "0,3,0,5,3,-4,0,-4,1,0,0,-5,2,-2,0,-4,3,-3", "1,3,-3,0,2,0,-5,4,-1,2,-2,-5,0,1,2,-1,3,-4,2,-3,0,1,-1" }, new string[3] { "A,B,C,A,C,B,C,A,B,C,B", "A,B,C,B,A,B,C,A,B,C,A,C", "A,B,A,B,C,A,B,C,B,A,C" }, 430f, 630f, 530f, true, false), new Obelisk(285f, 8, 2.45f, new string[3] { "1,4,2,5,1-4,5,1,3,2-5", "3,1,2-5,4,2,3,1-4,2,5", "4,1,2,1-4,3,5,1,3,2-5" }, 20f, 3f, 300f, new string[0], new string[0], 485f, new string[1] { "R,P,R,R,P" }, new string[2] { "45,300,40,290,50,310", "295,40,310,50,305,45" }, true, false, 5f), new Scan(1f, 0f, 0f, new string[1] { "9999-9999" }, 0f, new string[1] { "1" }, "R,R,R,R,P,R,R,R,R,R,P", 1f, true), new Bomb(0f, 0f, 0f, 0f, 0f, new string[0], 0f), new Main(0f), new Skull(new MinMax(2.5f, 5.4f), 575f, 2), new Bullets(635f, new string[5] { "5,4,4", "3,4,6", "4,3,6", "4,5,4", "3,6,4" }, 0.55f, 325f, 44f, 4, 0.65f, 335f, new MinMax(4f, 6f), 1.75f, 4, 0.6f, 225f, new MinMax(2.75f, 2.75f), "R,R,R,P", 145f, 100f)));
				break;
			}
			return new FlyingGenie(hp, goalTimes, list.ToArray());
		}
	}

	public class FlyingMermaid : AbstractLevelProperties<FlyingMermaid.State, FlyingMermaid.Pattern, FlyingMermaid.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected FlyingMermaid properties { get; private set; }

			public virtual void LevelInit(FlyingMermaid properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			Merdusa = 2,
			Head = 3
		}

		public enum Pattern
		{
			Yell = 0,
			Summon = 1,
			Fish = 2,
			Zap = 3,
			Eel = 4,
			Bubble = 5,
			HeadBlast = 6,
			BubbleHeadBlast = 7,
			Uninitialized = 8
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Yell yell;

			public readonly Summon summon;

			public readonly Seahorse seahorse;

			public readonly Pufferfish pufferfish;

			public readonly Turtle turtle;

			public readonly Fish fish;

			public readonly SpreadshotFish spreadshotFish;

			public readonly SpinnerFish spinnerFish;

			public readonly HomerFish homerFish;

			public readonly Eel eel;

			public readonly Zap zap;

			public readonly Bubbles bubbles;

			public readonly HeadBlast headBlast;

			public readonly Coral coral;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Yell yell, Summon summon, Seahorse seahorse, Pufferfish pufferfish, Turtle turtle, Fish fish, SpreadshotFish spreadshotFish, SpinnerFish spinnerFish, HomerFish homerFish, Eel eel, Zap zap, Bubbles bubbles, HeadBlast headBlast, Coral coral)
				: base(healthTrigger, patterns, stateName)
			{
				this.yell = yell;
				this.summon = summon;
				this.seahorse = seahorse;
				this.pufferfish = pufferfish;
				this.turtle = turtle;
				this.fish = fish;
				this.spreadshotFish = spreadshotFish;
				this.spinnerFish = spinnerFish;
				this.homerFish = homerFish;
				this.eel = eel;
				this.zap = zap;
				this.bubbles = bubbles;
				this.headBlast = headBlast;
				this.coral = coral;
			}
		}

		public class Yell : AbstractLevelPropertyGroup
		{
			public readonly string[] patternString;

			public readonly float anticipateInitialHold;

			public readonly float mouthHold;

			public readonly MinMax spreadAngle;

			public readonly int numBullets;

			public readonly float bulletSpeed;

			public readonly float anticipateHold;

			public readonly float hesitateAfterAttack;

			public Yell(string[] patternString, float anticipateInitialHold, float mouthHold, MinMax spreadAngle, int numBullets, float bulletSpeed, float anticipateHold, float hesitateAfterAttack)
			{
				this.patternString = patternString;
				this.anticipateInitialHold = anticipateInitialHold;
				this.mouthHold = mouthHold;
				this.spreadAngle = spreadAngle;
				this.numBullets = numBullets;
				this.bulletSpeed = bulletSpeed;
				this.anticipateHold = anticipateHold;
				this.hesitateAfterAttack = hesitateAfterAttack;
			}
		}

		public class Summon : AbstractLevelPropertyGroup
		{
			public readonly float holdBeforeCreature;

			public readonly float holdAfterCreature;

			public readonly float hesitateAfterAttack;

			public Summon(float holdBeforeCreature, float holdAfterCreature, float hesitateAfterAttack)
			{
				this.holdBeforeCreature = holdBeforeCreature;
				this.holdAfterCreature = holdAfterCreature;
				this.hesitateAfterAttack = hesitateAfterAttack;
			}
		}

		public class Seahorse : AbstractLevelPropertyGroup
		{
			public readonly float hp;

			public readonly float maxSpeed;

			public readonly float acceleration;

			public readonly float bounceRatio;

			public readonly float waterForce;

			public readonly float homingDuration;

			public Seahorse(float hp, float maxSpeed, float acceleration, float bounceRatio, float waterForce, float homingDuration)
			{
				this.hp = hp;
				this.maxSpeed = maxSpeed;
				this.acceleration = acceleration;
				this.bounceRatio = bounceRatio;
				this.waterForce = waterForce;
				this.homingDuration = homingDuration;
			}
		}

		public class Pufferfish : AbstractLevelPropertyGroup
		{
			public readonly float hp;

			public readonly float floatSpeed;

			public readonly float delay;

			public readonly float spawnDuration;

			public readonly string[] spawnString;

			public readonly MinMax pinkPufferSpawnRange;

			public Pufferfish(float hp, float floatSpeed, float delay, float spawnDuration, string[] spawnString, MinMax pinkPufferSpawnRange)
			{
				this.hp = hp;
				this.floatSpeed = floatSpeed;
				this.delay = delay;
				this.spawnDuration = spawnDuration;
				this.spawnString = spawnString;
				this.pinkPufferSpawnRange = pinkPufferSpawnRange;
			}
		}

		public class Turtle : AbstractLevelPropertyGroup
		{
			public readonly float hp;

			public readonly MinMax appearPosition;

			public readonly float speed;

			public readonly float bulletSpeed;

			public readonly MinMax timeUntilShoot;

			public readonly MinMax bulletTimeToExplode;

			public readonly float spreadshotBulletSpeed;

			public readonly string[] explodeSpreadshotString;

			public readonly float spiralRate;

			public Turtle(float hp, MinMax appearPosition, float speed, float bulletSpeed, MinMax timeUntilShoot, MinMax bulletTimeToExplode, float spreadshotBulletSpeed, string[] explodeSpreadshotString, float spiralRate)
			{
				this.hp = hp;
				this.appearPosition = appearPosition;
				this.speed = speed;
				this.bulletSpeed = bulletSpeed;
				this.timeUntilShoot = timeUntilShoot;
				this.bulletTimeToExplode = bulletTimeToExplode;
				this.spreadshotBulletSpeed = spreadshotBulletSpeed;
				this.explodeSpreadshotString = explodeSpreadshotString;
				this.spiralRate = spiralRate;
			}
		}

		public class Fish : AbstractLevelPropertyGroup
		{
			public readonly float delayBeforeFirstAttack;

			public readonly float delayBeforeFly;

			public readonly float flyingSpeed;

			public readonly float flyingUpSpeed;

			public readonly float flyingGravity;

			public readonly float hesitateAfterAttack;

			public Fish(float delayBeforeFirstAttack, float delayBeforeFly, float flyingSpeed, float flyingUpSpeed, float flyingGravity, float hesitateAfterAttack)
			{
				this.delayBeforeFirstAttack = delayBeforeFirstAttack;
				this.delayBeforeFly = delayBeforeFly;
				this.flyingSpeed = flyingSpeed;
				this.flyingUpSpeed = flyingUpSpeed;
				this.flyingGravity = flyingGravity;
				this.hesitateAfterAttack = hesitateAfterAttack;
			}
		}

		public class SpreadshotFish : AbstractLevelPropertyGroup
		{
			public readonly float attackDelay;

			public readonly string[] spreadVariableGroups;

			public readonly string[] shootString;

			public SpreadshotFish(float attackDelay, string[] spreadVariableGroups, string[] shootString)
			{
				this.attackDelay = attackDelay;
				this.spreadVariableGroups = spreadVariableGroups;
				this.shootString = shootString;
			}
		}

		public class SpinnerFish : AbstractLevelPropertyGroup
		{
			public readonly float bulletSpeed;

			public readonly float timeBeforeTails;

			public readonly float rotationSpeed;

			public readonly float attackDelay;

			public readonly string[] shootString;

			public SpinnerFish(float bulletSpeed, float timeBeforeTails, float rotationSpeed, float attackDelay, string[] shootString)
			{
				this.bulletSpeed = bulletSpeed;
				this.timeBeforeTails = timeBeforeTails;
				this.rotationSpeed = rotationSpeed;
				this.attackDelay = attackDelay;
				this.shootString = shootString;
			}
		}

		public class HomerFish : AbstractLevelPropertyGroup
		{
			public readonly float initSpeed;

			public readonly float timeBeforeHoming;

			public readonly float bulletSpeed;

			public readonly float rotationSpeed;

			public readonly float timeBeforeDeath;

			public readonly float attackDelay;

			public readonly string[] shootString;

			public HomerFish(float initSpeed, float timeBeforeHoming, float bulletSpeed, float rotationSpeed, float timeBeforeDeath, float attackDelay, string[] shootString)
			{
				this.initSpeed = initSpeed;
				this.timeBeforeHoming = timeBeforeHoming;
				this.bulletSpeed = bulletSpeed;
				this.rotationSpeed = rotationSpeed;
				this.timeBeforeDeath = timeBeforeDeath;
				this.attackDelay = attackDelay;
				this.shootString = shootString;
			}
		}

		public class Eel : AbstractLevelPropertyGroup
		{
			public readonly float hp;

			public readonly MinMax attackAmount;

			public readonly MinMax idleTime;

			public readonly MinMax appearDelay;

			public readonly MinMax spreadAngle;

			public readonly float numBullets;

			public readonly float bulletSpeed;

			public readonly float hesitateAfterAttack;

			public readonly string bulletPinkString;

			public Eel(float hp, MinMax attackAmount, MinMax idleTime, MinMax appearDelay, MinMax spreadAngle, float numBullets, float bulletSpeed, float hesitateAfterAttack, string bulletPinkString)
			{
				this.hp = hp;
				this.attackAmount = attackAmount;
				this.idleTime = idleTime;
				this.appearDelay = appearDelay;
				this.spreadAngle = spreadAngle;
				this.numBullets = numBullets;
				this.bulletSpeed = bulletSpeed;
				this.hesitateAfterAttack = hesitateAfterAttack;
				this.bulletPinkString = bulletPinkString;
			}
		}

		public class Zap : AbstractLevelPropertyGroup
		{
			public readonly float attackTime;

			public readonly MinMax hesitateAfterAttack;

			public readonly float stoneTime;

			public Zap(float attackTime, MinMax hesitateAfterAttack, float stoneTime)
			{
				this.attackTime = attackTime;
				this.hesitateAfterAttack = hesitateAfterAttack;
				this.stoneTime = stoneTime;
			}
		}

		public class Bubbles : AbstractLevelPropertyGroup
		{
			public readonly float movementSpeed;

			public readonly float waveSpeed;

			public readonly float waveAmount;

			public readonly float hp;

			public readonly MinMax attackDelayRange;

			public Bubbles(float movementSpeed, float waveSpeed, float waveAmount, float hp, MinMax attackDelayRange)
			{
				this.movementSpeed = movementSpeed;
				this.waveSpeed = waveSpeed;
				this.waveAmount = waveAmount;
				this.hp = hp;
				this.attackDelayRange = attackDelayRange;
			}
		}

		public class HeadBlast : AbstractLevelPropertyGroup
		{
			public readonly float movementSpeed;

			public readonly MinMax attackDelayRange;

			public HeadBlast(float movementSpeed, MinMax attackDelayRange)
			{
				this.movementSpeed = movementSpeed;
				this.attackDelayRange = attackDelayRange;
			}
		}

		public class Coral : AbstractLevelPropertyGroup
		{
			public readonly float coralMoveSpeed;

			public readonly string[] yellowDotPosString;

			public readonly MinMax yellowSpawnDelayRange;

			public readonly MinMax bubbleEyewaveSpawnDelayRange;

			public Coral(float coralMoveSpeed, string[] yellowDotPosString, MinMax yellowSpawnDelayRange, MinMax bubbleEyewaveSpawnDelayRange)
			{
				this.coralMoveSpeed = coralMoveSpeed;
				this.yellowDotPosString = yellowDotPosString;
				this.yellowSpawnDelayRange = yellowSpawnDelayRange;
				this.bubbleEyewaveSpawnDelayRange = bubbleEyewaveSpawnDelayRange;
			}
		}

		[CompilerGenerated]
		private static Dictionary<string, int> _003C_003Ef__switch_0024map9;

		public FlyingMermaid(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1900f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.75f));
				timeline.events.Add(new Level.Timeline.Event("Merdusa", 0.35f));
				break;
			case Level.Mode.Normal:
				timeline.health = 2500f;
				timeline.events.Add(new Level.Timeline.Event("Merdusa", 0.55f));
				timeline.events.Add(new Level.Timeline.Event("Head", 0.3f));
				break;
			case Level.Mode.Hard:
				timeline.health = 3000f;
				timeline.events.Add(new Level.Timeline.Event("Merdusa", 0.6f));
				timeline.events.Add(new Level.Timeline.Event("Head", 0.3f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null)
			{
				if (_003C_003Ef__switch_0024map9 == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(8);
					dictionary.Add("Y", 0);
					dictionary.Add("S", 1);
					dictionary.Add("F", 2);
					dictionary.Add("Z", 3);
					dictionary.Add("E", 4);
					dictionary.Add("B", 5);
					dictionary.Add("H", 6);
					dictionary.Add("D", 7);
					_003C_003Ef__switch_0024map9 = dictionary;
				}
				int value;
				if (_003C_003Ef__switch_0024map9.TryGetValue(id, out value))
				{
					switch (value)
					{
					case 0:
						return Pattern.Yell;
					case 1:
						return Pattern.Summon;
					case 2:
						return Pattern.Fish;
					case 3:
						return Pattern.Zap;
					case 4:
						return Pattern.Eel;
					case 5:
						return Pattern.Bubble;
					case 6:
						return Pattern.HeadBlast;
					case 7:
						return Pattern.BubbleHeadBlast;
					}
				}
			}
			Debug.LogError("Pattern FlyingMermaid.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static FlyingMermaid GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1900;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Yell,
					Pattern.Summon
				} }, States.Main, new Yell(new string[2] { "Y1,D0.6,Y1,D0.8,Y1", "Y1,D0.8,Y1,D0.6,Y1" }, 0.7f, 0.7f, new MinMax(140f, 240f), 3, 1100f, 0.25f, 0f), new Summon(0.25f, 1f, 0f), new Seahorse(80f, 550f, 1000f, 1.2f, 390f, 6.2f), new Pufferfish(10f, 300f, 1.25f, 6.4f, new string[4] { "50-850-450,250-700", "50,200,350,500,650,800,650,500,350,200", "100-800,300-600,450,300-600", "850,700,550,400,250,100,250,400,550,700" }, new MinMax(5f, 7f)), new Turtle(140f, new MinMax(950f, 1150f), 210f, 850f, new MinMax(0.9f, 1.7f), new MinMax(0.3f, 0.55f), 400f, new string[3] { "0-90-180-270,D0.5,0-90-180-270,D0.6,0-90-180-270", "0-90-180-270,D0.6,0-90-180-270,D0.7,0-90-180-270", "0-90-180-270,D0.7,0-90-180-270,D0.7,0-90-180-270" }, 0f), new Fish(1f, 2.3f, 900f, 300f, 3200f, 0f), new SpreadshotFish(0.5f, new string[6] { "S500,N5,100-220", "S500,N4,130-190", "S500,N5,90-210", "S500,N4,120-180", "S500,N5,80-200", "S500,N4,110-170" }, new string[6] { "S1,S2", "S3,S4", "S2,S1", "S5,S6", "S4,S3", "S6,S5" }), new SpinnerFish(300f, 0.3f, 150f, 1.8f, new string[1] { "S,S" }), new HomerFish(750f, 0.75f, 385f, 2.8f, 4.4f, 0.4f, new string[1] { "S" }), new Eel(40f, new MinMax(0f, 0f), new MinMax(1.2f, 2.3f), new MinMax(3f, 7f), new MinMax(110f, 195f), 4f, 460f, 0.1f, "R"), new Zap(0.5f, new MinMax(5f, 9f), 2.5f), new Bubbles(0f, 0f, 0f, 0f, new MinMax(0f, 1f)), new HeadBlast(0f, new MinMax(0f, 1f)), new Coral(0f, new string[0], new MinMax(0f, 1f), new MinMax(0f, 1f))));
				list.Add(new State(0.75f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Fish,
					Pattern.Summon
				} }, States.Generic, new Yell(new string[2] { "Y1,D0.6,Y1,D0.8,Y1", "Y1,D0.8,Y1,D0.6,Y1" }, 0.7f, 0.7f, new MinMax(140f, 240f), 3, 1100f, 0.25f, 0f), new Summon(0.25f, 1f, 0f), new Seahorse(80f, 550f, 1000f, 1.2f, 390f, 6.2f), new Pufferfish(10f, 300f, 1.25f, 6.4f, new string[4] { "50-850-450,250-700", "50,200,350,500,650,800,650,500,350,200", "100-800,300-600,450,300-600", "850,700,550,400,250,100,250,400,550,700" }, new MinMax(5f, 7f)), new Turtle(140f, new MinMax(950f, 1150f), 210f, 850f, new MinMax(0.9f, 1.7f), new MinMax(0.3f, 0.55f), 400f, new string[3] { "0-90-180-270,D0.5,0-90-180-270,D0.6,0-90-180-270", "0-90-180-270,D0.6,0-90-180-270,D0.7,0-90-180-270", "0-90-180-270,D0.7,0-90-180-270,D0.7,0-90-180-270" }, 0f), new Fish(1f, 2.3f, 900f, 300f, 3200f, 0f), new SpreadshotFish(0.5f, new string[6] { "S500,N5,100-220", "S500,N4,130-190", "S500,N5,90-210", "S500,N4,120-180", "S500,N5,80-200", "S500,N4,110-170" }, new string[6] { "S1,S2", "S3,S4", "S2,S1", "S5,S6", "S4,S3", "S6,S5" }), new SpinnerFish(300f, 0.3f, 150f, 1.8f, new string[1] { "S,S" }), new HomerFish(750f, 0.75f, 385f, 2.8f, 4.4f, 0.4f, new string[1] { "S" }), new Eel(40f, new MinMax(0f, 0f), new MinMax(1.2f, 2.3f), new MinMax(3f, 7f), new MinMax(110f, 195f), 4f, 460f, 0.1f, "R"), new Zap(0.5f, new MinMax(5f, 9f), 2.5f), new Bubbles(0f, 0f, 0f, 0f, new MinMax(0f, 1f)), new HeadBlast(0f, new MinMax(0f, 1f)), new Coral(0f, new string[0], new MinMax(0f, 1f), new MinMax(0f, 1f))));
				list.Add(new State(0.35f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Eel,
					Pattern.Zap
				} }, States.Merdusa, new Yell(new string[2] { "Y1,D0.6,Y1,D0.8,Y1", "Y1,D0.8,Y1,D0.6,Y1" }, 0.7f, 0.7f, new MinMax(140f, 240f), 3, 1100f, 0.25f, 0f), new Summon(0.25f, 1f, 0f), new Seahorse(80f, 550f, 1000f, 1.2f, 390f, 6.2f), new Pufferfish(10f, 300f, 1.25f, 6.4f, new string[4] { "50-850-450,250-700", "50,200,350,500,650,800,650,500,350,200", "100-800,300-600,450,300-600", "850,700,550,400,250,100,250,400,550,700" }, new MinMax(5f, 7f)), new Turtle(140f, new MinMax(950f, 1150f), 210f, 850f, new MinMax(0.9f, 1.7f), new MinMax(0.3f, 0.55f), 400f, new string[3] { "0-90-180-270,D0.5,0-90-180-270,D0.6,0-90-180-270", "0-90-180-270,D0.6,0-90-180-270,D0.7,0-90-180-270", "0-90-180-270,D0.7,0-90-180-270,D0.7,0-90-180-270" }, 0f), new Fish(1f, 2.3f, 900f, 300f, 3200f, 0f), new SpreadshotFish(0.5f, new string[6] { "S500,N5,100-220", "S500,N4,130-190", "S500,N5,90-210", "S500,N4,120-180", "S500,N5,80-200", "S500,N4,110-170" }, new string[6] { "S1,S2", "S3,S4", "S2,S1", "S5,S6", "S4,S3", "S6,S5" }), new SpinnerFish(300f, 0.3f, 150f, 1.8f, new string[1] { "S,S" }), new HomerFish(750f, 0.75f, 385f, 2.8f, 4.4f, 0.4f, new string[1] { "S" }), new Eel(40f, new MinMax(0f, 0f), new MinMax(1.2f, 2.3f), new MinMax(3f, 7f), new MinMax(110f, 195f), 4f, 460f, 0.1f, "R"), new Zap(0.5f, new MinMax(5f, 9f), 2.5f), new Bubbles(0f, 0f, 0f, 0f, new MinMax(0f, 1f)), new HeadBlast(0f, new MinMax(0f, 1f)), new Coral(0f, new string[0], new MinMax(0f, 1f), new MinMax(0f, 1f))));
				break;
			case Level.Mode.Normal:
				hp = 2500;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[4]
				{
					Pattern.Yell,
					Pattern.Summon,
					Pattern.Fish,
					Pattern.Summon
				} }, States.Main, new Yell(new string[1] { "Y1,D0.6,Y1,D0.6,Y1" }, 0.5f, 0.5f, new MinMax(140f, 240f), 3, 1300f, 0.25f, 0f), new Summon(0.25f, 1f, 0f), new Seahorse(90f, 700f, 1100f, 0.7f, 435f, 6.2f), new Pufferfish(10f, 300f, 1f, 8f, new string[3] { "50-400-750,150-500-850,100-450-800", "50-750,150-650,250-550,350-450,250-550,150-650", "50-750-400,250-600,100-800-450,250-600" }, new MinMax(6f, 8f)), new Turtle(150f, new MinMax(925f, 1100f), 200f, 850f, new MinMax(0.9f, 1.7f), new MinMax(0.3f, 0.55f), 415f, new string[3] { "350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.7,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305,D0.5,350-35-80-125-180-225-260-305" }, 0f), new Fish(1f, 2.1f, 1000f, 300f, 3200f, 0f), new SpreadshotFish(0.4f, new string[6] { "S600,N6,100-210", "S600,N6,110-220", "S600,N6,120-230", "S550,N6,90-200", "S550,N6,100-210", "S550,N6,110-220" }, new string[4] { "S1,S2,S3", "S4,S5,S6", "S3,S2,S1", "S6,S5,S4" }), new SpinnerFish(330f, 0.3f, 185f, 1f, new string[2] { "S,S,D1.5,S", "S,D1.5,S,S" }), new HomerFish(900f, 0.78f, 440f, 3.1f, 5.2f, 0.4f, new string[1] { "S" }), new Eel(45f, new MinMax(0f, 0f), new MinMax(1.2f, 2.7f), new MinMax(3f, 8f), new MinMax(110f, 215f), 5f, 500f, 0.1f, "R"), new Zap(0.5f, new MinMax(4f, 8f), 2.3f), new Bubbles(205f, 3f, 5f, 1f, new MinMax(1.5f, 2.3f)), new HeadBlast(745f, new MinMax(3.9f, 4.5f)), new Coral(335f, new string[7] { "0,60,120", "-60,-120", "0,60", "60,120", "0,-60,-120", "0,-60", "-60,-120" }, new MinMax(3f, 4.5f), new MinMax(4f, 5f))));
				list.Add(new State(0.55f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Eel,
					Pattern.Zap
				} }, States.Merdusa, new Yell(new string[1] { "Y1,D0.6,Y1,D0.6,Y1" }, 0.5f, 0.5f, new MinMax(140f, 240f), 3, 1300f, 0.25f, 0f), new Summon(0.25f, 1f, 0f), new Seahorse(90f, 700f, 1100f, 0.7f, 435f, 6.2f), new Pufferfish(10f, 300f, 1f, 8f, new string[3] { "50-400-750,150-500-850,100-450-800", "50-750,150-650,250-550,350-450,250-550,150-650", "50-750-400,250-600,100-800-450,250-600" }, new MinMax(6f, 8f)), new Turtle(150f, new MinMax(925f, 1100f), 200f, 850f, new MinMax(0.9f, 1.7f), new MinMax(0.3f, 0.55f), 415f, new string[3] { "350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.7,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305,D0.5,350-35-80-125-180-225-260-305" }, 0f), new Fish(1f, 2.1f, 1000f, 300f, 3200f, 0f), new SpreadshotFish(0.4f, new string[6] { "S600,N6,100-210", "S600,N6,110-220", "S600,N6,120-230", "S550,N6,90-200", "S550,N6,100-210", "S550,N6,110-220" }, new string[4] { "S1,S2,S3", "S4,S5,S6", "S3,S2,S1", "S6,S5,S4" }), new SpinnerFish(330f, 0.3f, 185f, 1f, new string[2] { "S,S,D1.5,S", "S,D1.5,S,S" }), new HomerFish(900f, 0.78f, 440f, 3.1f, 5.2f, 0.4f, new string[1] { "S" }), new Eel(45f, new MinMax(0f, 0f), new MinMax(1.2f, 2.7f), new MinMax(3f, 8f), new MinMax(110f, 215f), 5f, 500f, 0.1f, "R"), new Zap(0.5f, new MinMax(4f, 8f), 2.3f), new Bubbles(205f, 3f, 5f, 1f, new MinMax(1.5f, 2.3f)), new HeadBlast(745f, new MinMax(3.9f, 4.5f)), new Coral(335f, new string[7] { "0,60,120", "-60,-120", "0,60", "60,120", "0,-60,-120", "0,-60", "-60,-120" }, new MinMax(3f, 4.5f), new MinMax(4f, 5f))));
				list.Add(new State(0.3f, new Pattern[1][] { new Pattern[12]
				{
					Pattern.Bubble,
					Pattern.HeadBlast,
					Pattern.Bubble,
					Pattern.BubbleHeadBlast,
					Pattern.Bubble,
					Pattern.Bubble,
					Pattern.BubbleHeadBlast,
					Pattern.Bubble,
					Pattern.BubbleHeadBlast,
					Pattern.Bubble,
					Pattern.Bubble,
					Pattern.BubbleHeadBlast
				} }, States.Head, new Yell(new string[1] { "Y1,D0.6,Y1,D0.6,Y1" }, 0.5f, 0.5f, new MinMax(140f, 240f), 3, 1300f, 0.25f, 0f), new Summon(0.25f, 1f, 0f), new Seahorse(90f, 700f, 1100f, 0.7f, 435f, 6.2f), new Pufferfish(10f, 300f, 1f, 8f, new string[3] { "50-400-750,150-500-850,100-450-800", "50-750,150-650,250-550,350-450,250-550,150-650", "50-750-400,250-600,100-800-450,250-600" }, new MinMax(6f, 8f)), new Turtle(150f, new MinMax(925f, 1100f), 200f, 850f, new MinMax(0.9f, 1.7f), new MinMax(0.3f, 0.55f), 415f, new string[3] { "350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.7,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305,D0.5,350-35-80-125-180-225-260-305" }, 0f), new Fish(1f, 2.1f, 1000f, 300f, 3200f, 0f), new SpreadshotFish(0.4f, new string[6] { "S600,N6,100-210", "S600,N6,110-220", "S600,N6,120-230", "S550,N6,90-200", "S550,N6,100-210", "S550,N6,110-220" }, new string[4] { "S1,S2,S3", "S4,S5,S6", "S3,S2,S1", "S6,S5,S4" }), new SpinnerFish(330f, 0.3f, 185f, 1f, new string[2] { "S,S,D1.5,S", "S,D1.5,S,S" }), new HomerFish(900f, 0.78f, 440f, 3.1f, 5.2f, 0.4f, new string[1] { "S" }), new Eel(45f, new MinMax(0f, 0f), new MinMax(1.2f, 2.7f), new MinMax(3f, 8f), new MinMax(110f, 215f), 5f, 500f, 0.1f, "R"), new Zap(0.5f, new MinMax(4f, 8f), 2.3f), new Bubbles(205f, 3f, 5f, 1f, new MinMax(1.5f, 2.3f)), new HeadBlast(745f, new MinMax(3.9f, 4.5f)), new Coral(335f, new string[7] { "0,60,120", "-60,-120", "0,60", "60,120", "0,-60,-120", "0,-60", "-60,-120" }, new MinMax(3f, 4.5f), new MinMax(4f, 5f))));
				break;
			case Level.Mode.Hard:
				hp = 3000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[4]
				{
					Pattern.Fish,
					Pattern.Summon,
					Pattern.Yell,
					Pattern.Summon
				} }, States.Main, new Yell(new string[1] { "Y1,D0.5,Y1,D0.5,Y1" }, 0.5f, 0.5f, new MinMax(140f, 240f), 3, 1600f, 0.25f, 0f), new Summon(0.25f, 1f, 0f), new Seahorse(100f, 800f, 1300f, 1f, 480f, 6.2f), new Pufferfish(10f, 320f, 1.1f, 8f, new string[3] { "50-300-550-800,175-425-675,275-525,175-425-675", "50-200-350,450-600-750,150-300-450,550-700-850", "50-850-450,150-300,50-850-450,550-700" }, new MinMax(7f, 10f)), new Turtle(180f, new MinMax(900f, 1050f), 200f, 900f, new MinMax(1f, 1.8f), new MinMax(0.4f, 0.65f), 445f, new string[3] { "350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.7,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.5,350-35-80-125-180-225-260-305,D0.5,350-35-80-125-180-225-260-305" }, 0f), new Fish(1f, 1.9f, 1150f, 300f, 3200f, 0f), new SpreadshotFish(0.1f, new string[6] { "S600,N6,100-220", "S700,N5,112-208", "S600,N6,90-210", "S700,N5,102-198", "S600,N6,80-200", "S700,N5,92-188" }, new string[3] { "S1,S2,S1,S2", "S3,S4,S3,S4", "S5,S6,S5,S6" }), new SpinnerFish(320f, 0.3f, 215f, 0.9f, new string[4] { "S,S,D1.5,S", "S,D1.4,S,S", "S,S,D1.3,S", "S,D1.6,S,S" }), new HomerFish(1000f, 0.78f, 475f, 3.3f, 2.6f, 1.5f, new string[1] { "S,S" }), new Eel(45f, new MinMax(0f, 1f), new MinMax(1.2f, 2.7f), new MinMax(3f, 7f), new MinMax(105f, 225f), 5f, 540f, 0.1f, "R"), new Zap(0.5f, new MinMax(3.5f, 7f), 2.3f), new Bubbles(205f, 3f, 5f, 1f, new MinMax(1f, 2f)), new HeadBlast(745f, new MinMax(3f, 4.1f)), new Coral(415f, new string[7] { "0,60,120", "-60,-120", "0,60", "60,120", "0,-60,-120", "0,-60", "-60,-120" }, new MinMax(2.5f, 3.8f), new MinMax(3f, 4.1f))));
				list.Add(new State(0.6f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Eel,
					Pattern.Zap
				} }, States.Merdusa, new Yell(new string[1] { "Y1,D0.5,Y1,D0.5,Y1" }, 0.5f, 0.5f, new MinMax(140f, 240f), 3, 1600f, 0.25f, 0f), new Summon(0.25f, 1f, 0f), new Seahorse(100f, 800f, 1300f, 1f, 480f, 6.2f), new Pufferfish(10f, 320f, 1.1f, 8f, new string[3] { "50-300-550-800,175-425-675,275-525,175-425-675", "50-200-350,450-600-750,150-300-450,550-700-850", "50-850-450,150-300,50-850-450,550-700" }, new MinMax(7f, 10f)), new Turtle(180f, new MinMax(900f, 1050f), 200f, 900f, new MinMax(1f, 1.8f), new MinMax(0.4f, 0.65f), 445f, new string[3] { "350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.7,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.5,350-35-80-125-180-225-260-305,D0.5,350-35-80-125-180-225-260-305" }, 0f), new Fish(1f, 1.9f, 1150f, 300f, 3200f, 0f), new SpreadshotFish(0.1f, new string[6] { "S600,N6,100-220", "S700,N5,112-208", "S600,N6,90-210", "S700,N5,102-198", "S600,N6,80-200", "S700,N5,92-188" }, new string[3] { "S1,S2,S1,S2", "S3,S4,S3,S4", "S5,S6,S5,S6" }), new SpinnerFish(320f, 0.3f, 215f, 0.9f, new string[4] { "S,S,D1.5,S", "S,D1.4,S,S", "S,S,D1.3,S", "S,D1.6,S,S" }), new HomerFish(1000f, 0.78f, 475f, 3.3f, 2.6f, 1.5f, new string[1] { "S,S" }), new Eel(45f, new MinMax(0f, 1f), new MinMax(1.2f, 2.7f), new MinMax(3f, 7f), new MinMax(105f, 225f), 5f, 540f, 0.1f, "R"), new Zap(0.5f, new MinMax(3.5f, 7f), 2.3f), new Bubbles(205f, 3f, 5f, 1f, new MinMax(1f, 2f)), new HeadBlast(745f, new MinMax(3f, 4.1f)), new Coral(415f, new string[7] { "0,60,120", "-60,-120", "0,60", "60,120", "0,-60,-120", "0,-60", "-60,-120" }, new MinMax(2.5f, 3.8f), new MinMax(3f, 4.1f))));
				list.Add(new State(0.3f, new Pattern[1][] { new Pattern[13]
				{
					Pattern.BubbleHeadBlast,
					Pattern.Bubble,
					Pattern.BubbleHeadBlast,
					Pattern.Bubble,
					Pattern.Bubble,
					Pattern.BubbleHeadBlast,
					Pattern.Bubble,
					Pattern.Bubble,
					Pattern.BubbleHeadBlast,
					Pattern.Bubble,
					Pattern.BubbleHeadBlast,
					Pattern.Bubble,
					Pattern.BubbleHeadBlast
				} }, States.Head, new Yell(new string[1] { "Y1,D0.5,Y1,D0.5,Y1" }, 0.5f, 0.5f, new MinMax(140f, 240f), 3, 1600f, 0.25f, 0f), new Summon(0.25f, 1f, 0f), new Seahorse(100f, 800f, 1300f, 1f, 480f, 6.2f), new Pufferfish(10f, 320f, 1.1f, 8f, new string[3] { "50-300-550-800,175-425-675,275-525,175-425-675", "50-200-350,450-600-750,150-300-450,550-700-850", "50-850-450,150-300,50-850-450,550-700" }, new MinMax(7f, 10f)), new Turtle(180f, new MinMax(900f, 1050f), 200f, 900f, new MinMax(1f, 1.8f), new MinMax(0.4f, 0.65f), 445f, new string[3] { "350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.7,350-35-80-125-180-225-260-305,D0.6,350-35-80-125-180-225-260-305", "350-35-80-125-180-225-260-305,D0.5,350-35-80-125-180-225-260-305,D0.5,350-35-80-125-180-225-260-305" }, 0f), new Fish(1f, 1.9f, 1150f, 300f, 3200f, 0f), new SpreadshotFish(0.1f, new string[6] { "S600,N6,100-220", "S700,N5,112-208", "S600,N6,90-210", "S700,N5,102-198", "S600,N6,80-200", "S700,N5,92-188" }, new string[3] { "S1,S2,S1,S2", "S3,S4,S3,S4", "S5,S6,S5,S6" }), new SpinnerFish(320f, 0.3f, 215f, 0.9f, new string[4] { "S,S,D1.5,S", "S,D1.4,S,S", "S,S,D1.3,S", "S,D1.6,S,S" }), new HomerFish(1000f, 0.78f, 475f, 3.3f, 2.6f, 1.5f, new string[1] { "S,S" }), new Eel(45f, new MinMax(0f, 1f), new MinMax(1.2f, 2.7f), new MinMax(3f, 7f), new MinMax(105f, 225f), 5f, 540f, 0.1f, "R"), new Zap(0.5f, new MinMax(3.5f, 7f), 2.3f), new Bubbles(205f, 3f, 5f, 1f, new MinMax(1f, 2f)), new HeadBlast(745f, new MinMax(3f, 4.1f)), new Coral(415f, new string[7] { "0,60,120", "-60,-120", "0,60", "60,120", "0,-60,-120", "0,-60", "-60,-120" }, new MinMax(2.5f, 3.8f), new MinMax(3f, 4.1f))));
				break;
			}
			return new FlyingMermaid(hp, goalTimes, list.ToArray());
		}
	}

	public class FlyingTest : AbstractLevelProperties<FlyingTest.State, FlyingTest.Pattern, FlyingTest.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected FlyingTest properties { get; private set; }

			public virtual void LevelInit(FlyingTest properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Main = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public State(float healthTrigger, Pattern[][] patterns, States stateName)
				: base(healthTrigger, patterns, stateName)
			{
			}
		}

		public FlyingTest(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 200f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "M")
			{
				return Pattern.Main;
			}
			Debug.LogError("Pattern FlyingTest.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static FlyingTest GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main));
				break;
			case Level.Mode.Normal:
				hp = 200;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main));
				break;
			}
			return new FlyingTest(hp, goalTimes, list.ToArray());
		}
	}

	public class Frogs : AbstractLevelProperties<Frogs.State, Frogs.Pattern, Frogs.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Frogs properties { get; private set; }

			public virtual void LevelInit(Frogs properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			Roll = 2,
			Morph = 3
		}

		public enum Pattern
		{
			TallFan = 0,
			ShortRage = 1,
			TallFireflies = 2,
			ShortClap = 3,
			Morph = 4,
			RagePlusFireflies = 5,
			Uninitialized = 6
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly TallFan tallFan;

			public readonly TallFireflies tallFireflies;

			public readonly ShortRage shortRage;

			public readonly ShortRoll shortRoll;

			public readonly ShortClap shortClap;

			public readonly Morph morph;

			public readonly Demon demon;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, TallFan tallFan, TallFireflies tallFireflies, ShortRage shortRage, ShortRoll shortRoll, ShortClap shortClap, Morph morph, Demon demon)
				: base(healthTrigger, patterns, stateName)
			{
				this.tallFan = tallFan;
				this.tallFireflies = tallFireflies;
				this.shortRage = shortRage;
				this.shortRoll = shortRoll;
				this.shortClap = shortClap;
				this.morph = morph;
				this.demon = demon;
			}
		}

		public class TallFan : AbstractLevelPropertyGroup
		{
			public readonly float power;

			public readonly int accelerationTime;

			public readonly MinMax duration;

			public readonly int hesitate;

			public TallFan(float power, int accelerationTime, MinMax duration, int hesitate)
			{
				this.power = power;
				this.accelerationTime = accelerationTime;
				this.duration = duration;
				this.hesitate = hesitate;
			}
		}

		public class TallFireflies : AbstractLevelPropertyGroup
		{
			public readonly string[] patterns;

			public readonly float speed;

			public readonly float followTime;

			public readonly float followDelay;

			public readonly float followDistance;

			public readonly int hp;

			public readonly float hesitate;

			public readonly float invincibleDuration;

			public TallFireflies(string[] patterns, float speed, float followTime, float followDelay, float followDistance, int hp, float hesitate, float invincibleDuration)
			{
				this.patterns = patterns;
				this.speed = speed;
				this.followTime = followTime;
				this.followDelay = followDelay;
				this.followDistance = followDistance;
				this.hp = hp;
				this.hesitate = hesitate;
				this.invincibleDuration = invincibleDuration;
			}
		}

		public class ShortRage : AbstractLevelPropertyGroup
		{
			public readonly float anticipationDelay;

			public readonly float shotSpeed;

			public readonly float shotDelay;

			public readonly int shotCount;

			public readonly string[] parryPatterns;

			public readonly float hesitate;

			public ShortRage(float anticipationDelay, float shotSpeed, float shotDelay, int shotCount, string[] parryPatterns, float hesitate)
			{
				this.anticipationDelay = anticipationDelay;
				this.shotSpeed = shotSpeed;
				this.shotDelay = shotDelay;
				this.shotCount = shotCount;
				this.parryPatterns = parryPatterns;
				this.hesitate = hesitate;
			}
		}

		public class ShortRoll : AbstractLevelPropertyGroup
		{
			public readonly float delay;

			public readonly float time;

			public readonly float returnDelay;

			public readonly float hesitate;

			public ShortRoll(float delay, float time, float returnDelay, float hesitate)
			{
				this.delay = delay;
				this.time = time;
				this.returnDelay = returnDelay;
				this.hesitate = hesitate;
			}
		}

		public class ShortClap : AbstractLevelPropertyGroup
		{
			public readonly string[] patterns;

			public readonly float[] angles;

			public readonly float bulletSpeed;

			public readonly float shotDelay;

			public readonly float hesitate;

			public ShortClap(string[] patterns, float[] angles, float bulletSpeed, float shotDelay, float hesitate)
			{
				this.patterns = patterns;
				this.angles = angles;
				this.bulletSpeed = bulletSpeed;
				this.shotDelay = shotDelay;
				this.hesitate = hesitate;
			}
		}

		public class Morph : AbstractLevelPropertyGroup
		{
			public readonly float armDownDelay;

			public readonly float slotSelectionDurationPercentage;

			public readonly MinMax coinSpeed;

			public readonly MinMax coinDelay;

			public readonly float coinMinMaxTime;

			public readonly MinMax snakeSpeed;

			public readonly MinMax snakeDelay;

			public readonly float snakeMinMaxTime;

			public readonly float snakeDuration;

			public readonly MinMax bisonSpeed;

			public readonly MinMax bisonDelay;

			public readonly float bisonMinMaxTime;

			public readonly float bisonSmallX;

			public readonly float bisonBigX;

			public readonly int bisonDuration;

			public readonly float tigerSpeed;

			public readonly MinMax tigerDelay;

			public readonly float tigerMinMaxTime;

			public readonly float tigerDuration;

			public Morph(float armDownDelay, float slotSelectionDurationPercentage, MinMax coinSpeed, MinMax coinDelay, float coinMinMaxTime, MinMax snakeSpeed, MinMax snakeDelay, float snakeMinMaxTime, float snakeDuration, MinMax bisonSpeed, MinMax bisonDelay, float bisonMinMaxTime, float bisonSmallX, float bisonBigX, int bisonDuration, float tigerSpeed, MinMax tigerDelay, float tigerMinMaxTime, float tigerDuration)
			{
				this.armDownDelay = armDownDelay;
				this.slotSelectionDurationPercentage = slotSelectionDurationPercentage;
				this.coinSpeed = coinSpeed;
				this.coinDelay = coinDelay;
				this.coinMinMaxTime = coinMinMaxTime;
				this.snakeSpeed = snakeSpeed;
				this.snakeDelay = snakeDelay;
				this.snakeMinMaxTime = snakeMinMaxTime;
				this.snakeDuration = snakeDuration;
				this.bisonSpeed = bisonSpeed;
				this.bisonDelay = bisonDelay;
				this.bisonMinMaxTime = bisonMinMaxTime;
				this.bisonSmallX = bisonSmallX;
				this.bisonBigX = bisonBigX;
				this.bisonDuration = bisonDuration;
				this.tigerSpeed = tigerSpeed;
				this.tigerDelay = tigerDelay;
				this.tigerMinMaxTime = tigerMinMaxTime;
				this.tigerDuration = tigerDuration;
			}
		}

		public class Demon : AbstractLevelPropertyGroup
		{
			public readonly float demonFlameHeight;

			public readonly float demonParryHeight;

			public readonly MinMax demonSpeed;

			public readonly MinMax demonDelay;

			public readonly float demonMaxTime;

			public readonly string[] demonString;

			public Demon(float demonFlameHeight, float demonParryHeight, MinMax demonSpeed, MinMax demonDelay, float demonMaxTime, string[] demonString)
			{
				this.demonFlameHeight = demonFlameHeight;
				this.demonParryHeight = demonParryHeight;
				this.demonSpeed = demonSpeed;
				this.demonDelay = demonDelay;
				this.demonMaxTime = demonMaxTime;
				this.demonString = demonString;
			}
		}

		public Frogs(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1400f;
				timeline.events.Add(new Level.Timeline.Event("Roll", 0.61f));
				break;
			case Level.Mode.Normal:
				timeline.health = 1700f;
				timeline.events.Add(new Level.Timeline.Event("Roll", 0.76f));
				timeline.events.Add(new Level.Timeline.Event("Morph", 0.34f));
				break;
			case Level.Mode.Hard:
				timeline.health = 1900f;
				timeline.events.Add(new Level.Timeline.Event("Roll", 0.74f));
				timeline.events.Add(new Level.Timeline.Event("Morph", 0.35f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "F":
				return Pattern.TallFan;
			case "R":
				return Pattern.ShortRage;
			case "S":
				return Pattern.TallFireflies;
			case "C":
				return Pattern.ShortClap;
			case "M":
				return Pattern.Morph;
			case "P":
				return Pattern.RagePlusFireflies;
			default:
				Debug.LogError("Pattern Frogs.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static Frogs GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1400;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.TallFireflies,
					Pattern.ShortRage
				} }, States.Main, new TallFan(-225f, 2, new MinMax(3.5f, 4.5f), 2), new TallFireflies(new string[3] { "D:1, S:2, D:1.5, S:1, D:1, S:1", "D:1.5, S:1, D:1.5, S:1, D:1, S:2", "D:0.5, S:2, D:1, S:1, D:0.5, S:1" }, 1000f, 0.5f, 1.5f, 180f, 4, 2f, 0.15f), new ShortRage(1f, 800f, 0.8f, 5, new string[3] { "PRPPR", "RPPRP", "PRPRP" }, 1f), new ShortRoll(3f, 2f, 0.8f, 0.1f), new ShortClap(new string[5] { "S:1, D:2.8, S:1", "S:1, D:2, S:1", "S:1", "S:1, D:1.8, S:1", "S:1, D:2.5, S:1" }, new float[7] { 65f, 68f, 66f, 69f, 67f, 67f, 69f }, 850f, 1.5f, 2f), new Morph(0f, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, 0f, 0f, 0, 0f, new MinMax(0f, 1f), 0f, 0f), new Demon(0f, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new string[0])));
				list.Add(new State(0.61f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.ShortClap,
					Pattern.ShortRage
				} }, States.Roll, new TallFan(-225f, 2, new MinMax(3.5f, 4.5f), 2), new TallFireflies(new string[3] { "D:1, S:2, D:1.5, S:1, D:1, S:1", "D:1.5, S:1, D:1.5, S:1, D:1, S:2", "D:0.5, S:2, D:1, S:1, D:0.5, S:1" }, 1000f, 0.5f, 1.5f, 180f, 4, 2f, 0.15f), new ShortRage(1f, 800f, 1f, 3, new string[5] { "PRR", "RPR", "PRR", "RRP", "PRR" }, 2f), new ShortRoll(3f, 2f, 0.8f, 0.1f), new ShortClap(new string[5] { "S:1, D:2.8, S:1", "S:1, D:2, S:1", "S:1", "S:1, D:1.8, S:1", "S:1, D:2.5, S:1" }, new float[7] { 65f, 68f, 66f, 69f, 67f, 67f, 69f }, 850f, 1.5f, 2f), new Morph(0f, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, 0f, 0f, 0, 0f, new MinMax(0f, 1f), 0f, 0f), new Demon(0f, 0f, new MinMax(0f, 1f), new MinMax(0f, 1f), 0f, new string[0])));
				break;
			case Level.Mode.Normal:
				hp = 1700;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.ShortRage,
					Pattern.TallFireflies
				} }, States.Main, new TallFan(-300f, 2, new MinMax(3f, 4f), 1), new TallFireflies(new string[4] { "D:1, S:2, D:1, S:2, D:1, S:1, D:1, S:2", "D:1, S:2, D:1, S:2, D:1, S:2", "D:1, S:2, D:1, S:1, D:1, S:2, D:1, S:2", "D:1, S:2, D:1, S:2, D:1, S:1" }, 1000f, 0.4f, 1.3f, 210f, 4, 2f, 0.15f), new ShortRage(1f, 900f, 0.65f, 9, new string[3] { "RRP", "PRR", "RPR" }, 1f), new ShortRoll(2f, 1.7f, 0.8f, 0.1f), new ShortClap(new string[3] { "S:1, D:1.5, S:1, D:1.7, S:1", "S:1, D:1.7, S:1, D:1.5, S:1", "S:1, D:1.6, S:1, D:1.6, S:1" }, new float[9] { 72f, 68f, 70f, 69f, 66f, 71f, 68f, 70f, 67f }, 950f, 0.8f, 2f), new Morph(3f, 0.7f, new MinMax(700f, 950f), new MinMax(1.3f, 0.9f), 7f, new MinMax(600f, 900f), new MinMax(0.6f, 0.3f), 5f, 10f, new MinMax(500f, 700f), new MinMax(1.2f, 0.8f), 5f, 600f, 300f, 10, 478f, new MinMax(1.3f, 0.8f), 5f, 10f), new Demon(8f, 15f, new MinMax(480f, 620f), new MinMax(1.4f, 0.85f), 13f, new string[6] { "S,S,T,S,B,B,S,S,O,S,S,T", "S,S,O,S,T,B,S,S,B,B,S,O", "S,T,B,S,S,T,S,B,T,S,S,O", "S,B,B,S,S,S,T,O,S,S,B,S", "S,T,S,O,S,S,S,B,B,B,S,T", "S,S,T,S,B,B,S,S,O,S,B,S" })));
				list.Add(new State(0.76f, new Pattern[1][] { new Pattern[1] { Pattern.ShortClap } }, States.Roll, new TallFan(-300f, 2, new MinMax(3f, 4f), 1), new TallFireflies(new string[4] { "D:1, S:2, D:1, S:2, D:1, S:1, D:1, S:2", "D:1, S:2, D:1, S:2, D:1, S:2", "D:1, S:2, D:1, S:1, D:1, S:2, D:1, S:2", "D:1, S:2, D:1, S:2, D:1, S:1" }, 1000f, 0.4f, 1.3f, 210f, 4, 2f, 0.15f), new ShortRage(1f, 900f, 0.65f, 9, new string[3] { "RRP", "PRR", "RPR" }, 1f), new ShortRoll(2f, 1.7f, 0.8f, 0.1f), new ShortClap(new string[3] { "S:1, D:1.5, S:1, D:1.7, S:1", "S:1, D:1.7, S:1, D:1.5, S:1", "S:1, D:1.6, S:1, D:1.6, S:1" }, new float[9] { 72f, 68f, 70f, 69f, 66f, 71f, 68f, 70f, 67f }, 950f, 0.8f, 2f), new Morph(3f, 0.7f, new MinMax(700f, 950f), new MinMax(1.3f, 0.9f), 7f, new MinMax(600f, 900f), new MinMax(0.6f, 0.3f), 5f, 10f, new MinMax(500f, 700f), new MinMax(1.2f, 0.8f), 5f, 600f, 300f, 10, 478f, new MinMax(1.3f, 0.8f), 5f, 10f), new Demon(8f, 15f, new MinMax(480f, 620f), new MinMax(1.4f, 0.85f), 13f, new string[6] { "S,S,T,S,B,B,S,S,O,S,S,T", "S,S,O,S,T,B,S,S,B,B,S,O", "S,T,B,S,S,T,S,B,T,S,S,O", "S,B,B,S,S,S,T,O,S,S,B,S", "S,T,S,O,S,S,S,B,B,B,S,T", "S,S,T,S,B,B,S,S,O,S,B,S" })));
				list.Add(new State(0.34f, new Pattern[1][] { new Pattern[1] { Pattern.Morph } }, States.Morph, new TallFan(-300f, 2, new MinMax(3f, 4f), 1), new TallFireflies(new string[4] { "D:1, S:2, D:1, S:2, D:1, S:1, D:1, S:2", "D:1, S:2, D:1, S:2, D:1, S:2", "D:1, S:2, D:1, S:1, D:1, S:2, D:1, S:2", "D:1, S:2, D:1, S:2, D:1, S:1" }, 1000f, 0.4f, 1.3f, 210f, 4, 2f, 0.15f), new ShortRage(1f, 900f, 0.65f, 9, new string[3] { "RRP", "PRR", "RPR" }, 1f), new ShortRoll(2f, 1.7f, 0.8f, 0.1f), new ShortClap(new string[3] { "S:1, D:1.5, S:1, D:1.7, S:1", "S:1, D:1.7, S:1, D:1.5, S:1", "S:1, D:1.6, S:1, D:1.6, S:1" }, new float[9] { 72f, 68f, 70f, 69f, 66f, 71f, 68f, 70f, 67f }, 950f, 0.8f, 2f), new Morph(3f, 0.7f, new MinMax(700f, 950f), new MinMax(1.3f, 0.9f), 7f, new MinMax(600f, 900f), new MinMax(0.6f, 0.3f), 5f, 10f, new MinMax(500f, 700f), new MinMax(1.2f, 0.8f), 5f, 600f, 300f, 10, 478f, new MinMax(1.3f, 0.8f), 5f, 10f), new Demon(8f, 15f, new MinMax(480f, 620f), new MinMax(1.4f, 0.85f), 13f, new string[6] { "S,S,T,S,B,B,S,S,O,S,S,T", "S,S,O,S,T,B,S,S,B,B,S,O", "S,T,B,S,S,T,S,B,T,S,S,O", "S,B,B,S,S,S,T,O,S,S,B,S", "S,T,S,O,S,S,S,B,B,B,S,T", "S,S,T,S,B,B,S,S,O,S,B,S" })));
				break;
			case Level.Mode.Hard:
				hp = 1900;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] { Pattern.RagePlusFireflies } }, States.Main, new TallFan(-410f, 2, new MinMax(2.5f, 3.5f), 1), new TallFireflies(new string[7] { "D:1, S:2, D:2.5, S:2, D:1.5, S:1", "D:0.5, S:1, D:2, S:1, D:1.5, S:2", "D:1, S:2, D:2.2, S:2", "D:1, S:2, D:2, S:1, D:1, S:1", "D:0.5, S:1, D:2.5, S:2, D:2.1, S:2", "D:1.4, S:2, D:2.4, S:2", "D:1, S:1, D:1.8, S:2, D:1.5, S:1" }, 1000f, 0.4f, 1f, 180f, 4, 2f, 0.15f), new ShortRage(1f, 900f, 0.5f, 5, new string[3] { "RRPRP", "RPRPR", "PRRRP" }, 2f), new ShortRoll(1.5f, 1.5f, 0.8f, 0.1f), new ShortClap(new string[6] { "S:1, D:1.5, S:1, D:1.8, S:1", "S:1, D:1.7, S:1", "S:1, D:1.6, S:1, D:1.7, S:1", "S:1, D:2, S:1", "S:1, D:1.4, S:1, D:1.9, S:1", "S:1, D:2, S:1" }, new float[12]
				{
					73f, 68f, 70f, 75f, 76f, 69f, 74f, 70f, 67f, 77f,
					72f, 67f
				}, 1030f, 0.8f, 1f), new Morph(3f, 0.7f, new MinMax(800f, 1000f), new MinMax(0.7f, 0.5f), 5f, new MinMax(700f, 1150f), new MinMax(0.7f, 0.3f), 6f, 10f, new MinMax(600f, 920f), new MinMax(1f, 0.65f), 6f, 600f, 300f, 10, 488f, new MinMax(1.2f, 0.7f), 6f, 10f), new Demon(8f, 15f, new MinMax(530f, 690f), new MinMax(1.2f, 0.7f), 13f, new string[8] { "S,S,T,S,B,S,S,S,O,S,S,T", "S,T,B,S,S,O,S,B,T,S,S,B", "S,S,O,S,T,B,S,S,B,T,S,O", "S,S,B,S,B,S,S,T,S,O,S,S,B", "S,S,T,B,S,S,B,S,O,S,S,B", "S,S,T,S,B,O,S,S,B,S,S,T", "S,S,T,S,T,S,S,B,O,S,B,S", "S,S,O,S,T,S,S,B,B,S,S,O" })));
				list.Add(new State(0.74f, new Pattern[1][] { new Pattern[9]
				{
					Pattern.ShortRage,
					Pattern.ShortClap,
					Pattern.ShortRage,
					Pattern.ShortClap,
					Pattern.ShortRage,
					Pattern.ShortClap,
					Pattern.ShortRage,
					Pattern.ShortClap,
					Pattern.ShortClap
				} }, States.Roll, new TallFan(-410f, 2, new MinMax(2.5f, 3.5f), 1), new TallFireflies(new string[7] { "D:1, S:2, D:2.5, S:2, D:1.5, S:1", "D:0.5, S:1, D:2, S:1, D:1.5, S:2", "D:1, S:2, D:2.2, S:2", "D:1, S:2, D:2, S:1, D:1, S:1", "D:0.5, S:1, D:2.5, S:2, D:2.1, S:2", "D:1.4, S:2, D:2.4, S:2", "D:1, S:1, D:1.8, S:2, D:1.5, S:1" }, 1000f, 0.4f, 1f, 180f, 4, 2f, 0.15f), new ShortRage(1f, 850f, 0.6f, 3, new string[4] { "RRP", "PRR", "RPR", "PRR" }, 2f), new ShortRoll(1.5f, 1.5f, 0.8f, 0.1f), new ShortClap(new string[6] { "S:1, D:1.5, S:1, D:1.8, S:1", "S:1, D:1.7, S:1", "S:1, D:1.6, S:1, D:1.7, S:1", "S:1, D:2, S:1", "S:1, D:1.4, S:1, D:1.9, S:1", "S:1, D:2, S:1" }, new float[12]
				{
					73f, 68f, 70f, 75f, 76f, 69f, 74f, 70f, 67f, 77f,
					72f, 67f
				}, 1030f, 0.8f, 1f), new Morph(3f, 0.7f, new MinMax(800f, 1000f), new MinMax(0.7f, 0.5f), 5f, new MinMax(700f, 1150f), new MinMax(0.7f, 0.3f), 6f, 10f, new MinMax(600f, 920f), new MinMax(1f, 0.65f), 6f, 600f, 300f, 10, 488f, new MinMax(1.2f, 0.7f), 6f, 10f), new Demon(8f, 15f, new MinMax(530f, 690f), new MinMax(1.2f, 0.7f), 13f, new string[8] { "S,S,T,S,B,S,S,S,O,S,S,T", "S,T,B,S,S,O,S,B,T,S,S,B", "S,S,O,S,T,B,S,S,B,T,S,O", "S,S,B,S,B,S,S,T,S,O,S,S,B", "S,S,T,B,S,S,B,S,O,S,S,B", "S,S,T,S,B,O,S,S,B,S,S,T", "S,S,T,S,T,S,S,B,O,S,B,S", "S,S,O,S,T,S,S,B,B,S,S,O" })));
				list.Add(new State(0.35f, new Pattern[1][] { new Pattern[0] }, States.Morph, new TallFan(-410f, 2, new MinMax(2.5f, 3.5f), 1), new TallFireflies(new string[7] { "D:1, S:2, D:2.5, S:2, D:1.5, S:1", "D:0.5, S:1, D:2, S:1, D:1.5, S:2", "D:1, S:2, D:2.2, S:2", "D:1, S:2, D:2, S:1, D:1, S:1", "D:0.5, S:1, D:2.5, S:2, D:2.1, S:2", "D:1.4, S:2, D:2.4, S:2", "D:1, S:1, D:1.8, S:2, D:1.5, S:1" }, 1000f, 0.4f, 1f, 180f, 4, 2f, 0.15f), new ShortRage(1f, 850f, 0.6f, 3, new string[4] { "RRP", "PRR", "RPR", "PRR" }, 2f), new ShortRoll(1.5f, 1.5f, 0.8f, 0.1f), new ShortClap(new string[6] { "S:1, D:1.5, S:1, D:1.8, S:1", "S:1, D:1.7, S:1", "S:1, D:1.6, S:1, D:1.7, S:1", "S:1, D:2, S:1", "S:1, D:1.4, S:1, D:1.9, S:1", "S:1, D:2, S:1" }, new float[12]
				{
					73f, 68f, 70f, 75f, 76f, 69f, 74f, 70f, 67f, 77f,
					72f, 67f
				}, 1030f, 0.8f, 1f), new Morph(3f, 0.7f, new MinMax(800f, 1000f), new MinMax(0.7f, 0.5f), 5f, new MinMax(700f, 1150f), new MinMax(0.7f, 0.3f), 6f, 10f, new MinMax(600f, 920f), new MinMax(1f, 0.65f), 6f, 600f, 300f, 10, 488f, new MinMax(1.2f, 0.7f), 6f, 10f), new Demon(8f, 15f, new MinMax(530f, 690f), new MinMax(1.2f, 0.7f), 13f, new string[8] { "S,S,T,S,B,S,S,S,O,S,S,T", "S,T,B,S,S,O,S,B,T,S,S,B", "S,S,O,S,T,B,S,S,B,T,S,O", "S,S,B,S,B,S,S,T,S,O,S,S,B", "S,S,T,B,S,S,B,S,O,S,S,B", "S,S,T,S,B,O,S,S,B,S,S,T", "S,S,T,S,T,S,S,B,O,S,B,S", "S,S,O,S,T,S,S,B,B,S,S,O" })));
				break;
			}
			return new Frogs(hp, goalTimes, list.ToArray());
		}
	}

	public class House : AbstractLevelProperties<House.State, House.Pattern, House.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected House properties { get; private set; }

			public virtual void LevelInit(House properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public State(float healthTrigger, Pattern[][] patterns, States stateName)
				: base(healthTrigger, patterns, stateName)
			{
			}
		}

		public House(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 100f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern House.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static House GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			case Level.Mode.Normal:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			}
			return new House(hp, goalTimes, list.ToArray());
		}
	}

	public class Mausoleum : AbstractLevelProperties<Mausoleum.State, Mausoleum.Pattern, Mausoleum.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Mausoleum properties { get; private set; }

			public virtual void LevelInit(Mausoleum properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			WaveTwo = 2,
			WaveThree = 3
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Main main;

			public readonly RegularGhost regularGhost;

			public readonly CircleGhost circleGhost;

			public readonly BigGhost bigGhost;

			public readonly DelayGhost delayGhost;

			public readonly SineGhost sineGhost;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Main main, RegularGhost regularGhost, CircleGhost circleGhost, BigGhost bigGhost, DelayGhost delayGhost, SineGhost sineGhost)
				: base(healthTrigger, patterns, stateName)
			{
				this.main = main;
				this.regularGhost = regularGhost;
				this.circleGhost = circleGhost;
				this.bigGhost = bigGhost;
				this.delayGhost = delayGhost;
				this.sineGhost = sineGhost;
			}
		}

		public class Main : AbstractLevelPropertyGroup
		{
			public readonly string[] delayString;

			public readonly string[] spawnString;

			public readonly int ghostCount;

			public readonly string[] ghostTypeString;

			public Main(string[] delayString, string[] spawnString, int ghostCount, string[] ghostTypeString)
			{
				this.delayString = delayString;
				this.spawnString = spawnString;
				this.ghostCount = ghostCount;
				this.ghostTypeString = ghostTypeString;
			}
		}

		public class RegularGhost : AbstractLevelPropertyGroup
		{
			public readonly float speed;

			public readonly float multiDelay;

			public readonly float mainAddDelay;

			public RegularGhost(float speed, float multiDelay, float mainAddDelay)
			{
				this.speed = speed;
				this.multiDelay = multiDelay;
				this.mainAddDelay = mainAddDelay;
			}
		}

		public class CircleGhost : AbstractLevelPropertyGroup
		{
			public readonly float extraDelay;

			public readonly float circleSpeed;

			public readonly float circleRate;

			public CircleGhost(float extraDelay, float circleSpeed, float circleRate)
			{
				this.extraDelay = extraDelay;
				this.circleSpeed = circleSpeed;
				this.circleRate = circleRate;
			}
		}

		public class BigGhost : AbstractLevelPropertyGroup
		{
			public readonly float speed;

			public readonly float littleGhostSpeed;

			public readonly float multiDelay;

			public readonly float mainAddDelay;

			public BigGhost(float speed, float littleGhostSpeed, float multiDelay, float mainAddDelay)
			{
				this.speed = speed;
				this.littleGhostSpeed = littleGhostSpeed;
				this.multiDelay = multiDelay;
				this.mainAddDelay = mainAddDelay;
			}
		}

		public class DelayGhost : AbstractLevelPropertyGroup
		{
			public readonly float dashDelay;

			public readonly float speed;

			public DelayGhost(float dashDelay, float speed)
			{
				this.dashDelay = dashDelay;
				this.speed = speed;
			}
		}

		public class SineGhost : AbstractLevelPropertyGroup
		{
			public readonly float multiDelay;

			public readonly float mainAddDelay;

			public readonly float ghostSpeed;

			public readonly float waveSpeed;

			public readonly float waveAmount;

			public SineGhost(float multiDelay, float mainAddDelay, float ghostSpeed, float waveSpeed, float waveAmount)
			{
				this.multiDelay = multiDelay;
				this.mainAddDelay = mainAddDelay;
				this.ghostSpeed = ghostSpeed;
				this.waveSpeed = waveSpeed;
				this.waveAmount = waveAmount;
			}
		}

		public Mausoleum(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				timeline.events.Add(new Level.Timeline.Event("WaveTwo", 0.67f));
				timeline.events.Add(new Level.Timeline.Event("WaveThree", 0.34f));
				break;
			case Level.Mode.Normal:
				timeline.health = 100f;
				timeline.events.Add(new Level.Timeline.Event("WaveTwo", 0.67f));
				timeline.events.Add(new Level.Timeline.Event("WaveThree", 0.34f));
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				timeline.events.Add(new Level.Timeline.Event("WaveTwo", 0.67f));
				timeline.events.Add(new Level.Timeline.Event("WaveThree", 0.34f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern Mausoleum.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static Mausoleum GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Main(new string[3] { "1,1,1.5,1,1,2", "1,1.5,1,1,1,2", "1,1,1,1.5,1,2" }, new string[4] { "1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6", "1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6", "1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9", "7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3" }, 7, new string[1] { "R,R" }), new RegularGhost(160f, 0.8f, 0.9f), new CircleGhost(0f, 0f, 0f), new BigGhost(0f, 0f, 0f, 0f), new DelayGhost(0f, 0f), new SineGhost(0.6f, 1f, 240f, 6f, 3f)));
				list.Add(new State(0.67f, new Pattern[1][] { new Pattern[1] }, States.WaveTwo, new Main(new string[3] { "1.5,1.4,2", "1.4,1.5,2", "1.5,1.5,1.9" }, new string[6] { "1,9", "7,3", "4,6", "1,3", "7,9", "4,6" }, 7, new string[1] { "R-R,R,R-R,R,R" }), new RegularGhost(160f, 0.8f, 0.9f), new CircleGhost(0f, 0f, 0f), new BigGhost(0f, 0f, 0f, 0f), new DelayGhost(0f, 0f), new SineGhost(0.6f, 1f, 240f, 6f, 3f)));
				list.Add(new State(0.34f, new Pattern[1][] { new Pattern[1] }, States.WaveThree, new Main(new string[3] { "1.3,1.3,1.7,1.4,1.4,1.6,1.2,1.2,1.7", "1.3,1.2,1.7,1.4,1.3,1.6,1.2,1.4,1.7", "1.4,1.2,1.7,1.4,1.3,1.7,1.2,1.3,1.6" }, new string[4] { "4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7", "6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9", "9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7", "7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9" }, 7, new string[3] { "R,R,S,R,R,S,R,S", "S,R,R,R,S,R,S,R", "R,S,R,R,S,S,R,R" }), new RegularGhost(160f, 0.8f, 0.9f), new CircleGhost(0f, 0f, 0f), new BigGhost(0f, 0f, 0f, 0f), new DelayGhost(0f, 0f), new SineGhost(0.6f, 1f, 240f, 6f, 3f)));
				break;
			case Level.Mode.Normal:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Main(new string[3] { "1,0.8,1.3,1,0.8,1.7", "0.8,1.3,0.8,1,1,1.7", "1,1,0.8,1.3,0.8,1.7" }, new string[4] { "1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6", "1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6", "1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9", "7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3" }, 8, new string[3] { "R,R,S,R,R2,S,R,S", "R,S,R,S,R2,R,S,R", "S,R,R,S,R,R,S,R2" }), new RegularGhost(170f, 0.8f, 0.8f), new CircleGhost(0.6f, 85f, 0.3f), new BigGhost(120f, 200f, 0f, 2f), new DelayGhost(3f, 500f), new SineGhost(0.6f, 1f, 250f, 6f, 3f)));
				list.Add(new State(0.67f, new Pattern[1][] { new Pattern[0] }, States.WaveTwo, new Main(new string[2] { "1.6,1.8,2.2", "1.8,1.6,2.2" }, new string[6] { "1,9", "7,3", "4,6", "1,3", "7,9", "4,6" }, 8, new string[3] { "R-R,S,R,C,R,S,R2,C,R-R", "R,S,R-R,C,R,R2,S,R-R,C", "R,C,S,R,R2,C,R-R,S,R-R" }), new RegularGhost(170f, 0.8f, 0.8f), new CircleGhost(0.6f, 85f, 0.3f), new BigGhost(120f, 200f, 0f, 2f), new DelayGhost(3f, 500f), new SineGhost(0.6f, 1f, 250f, 6f, 3f)));
				list.Add(new State(0.34f, new Pattern[1][] { new Pattern[0] }, States.WaveThree, new Main(new string[1] { "1.3,1.3,1.7,1.4,1.4,1.7,1.2,1.2,1.7" }, new string[4] { "4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7", "6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9", "9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7", "7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9" }, 8, new string[2] { "C-C,R,R,C,S,R2,S,C,R,R2,C,S,R2,R", "R,C,R,S,C-C,R,R2,C,R2,S,R,C,R2,S" }), new RegularGhost(170f, 0.8f, 0.8f), new CircleGhost(0.6f, 85f, 0.3f), new BigGhost(120f, 200f, 0f, 2f), new DelayGhost(3f, 500f), new SineGhost(0.6f, 1f, 250f, 6f, 3f)));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Main(new string[3] { "1,1,1.5,1,0.8,1.6,0.8,1,1.7", "1,1.2,1.2,1,0.8,1.5,0.8,1.2,1.3", "1,1,1.4,0.9,0.9,1.6,1.1,1.1,1.4" }, new string[4] { "1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6,1,3,4,6,7,9,4,6", "1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6", "1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9,1,4,7,3,6,9", "7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3,7,4,1,9,6,3" }, 9, new string[3] { "R,S,R2,S,R-R-R,R2,S,R,S,R,S-S,R,R,S2", "R-S,R,S,S-S,R2,S,R,R,S2,R-R-R,S,R,S2", "R-R,S,R2,S-S,S,R,S2,R2,S,R-R-R,S2" }), new RegularGhost(190f, 0.7f, 0.4f), new CircleGhost(0.5f, 85f, 0.3f), new BigGhost(125f, 170f, 0f, 0.1f), new DelayGhost(0f, 0f), new SineGhost(0.6f, 0.6f, 250f, 6f, 3f)));
				list.Add(new State(0.67f, new Pattern[1][] { new Pattern[1] }, States.WaveTwo, new Main(new string[4] { "1.5,1.8,2,1.2,1.5,1.8", "1.6,1.7,2,1.3,1.4,1.9", "2,1.6,1.9,1.7,1.8,1.9", "1.6,1.9,1.5,2,1.6,1.8" }, new string[1] { "1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6" }, 9, new string[3] { "C-C,R,B,S,R-R,B,C,R2,R,B,C,S", "C,B,R2,S,B,C-C,R,R-R,B,C,S", "R2,B,S,R,R,B,C,S,R-R,B,C-C,R" }), new RegularGhost(190f, 0.7f, 0.4f), new CircleGhost(0.5f, 85f, 0.3f), new BigGhost(125f, 170f, 0f, 0.1f), new DelayGhost(0f, 0f), new SineGhost(0.6f, 0.6f, 250f, 6f, 3f)));
				list.Add(new State(0.34f, new Pattern[1][] { new Pattern[1] }, States.WaveThree, new Main(new string[4] { "2,1.8,1.6,2,1.8,1.9,1.8,2,1.6,2,1.7", "1.8,1.9,2,1.7,2,1.8,1.7,2,1.6,1.9,2", "1.7,1.7,1.9,1.6,2,1.8,1.8,1.7,2,1.9", "1.9,1.6,2,1.6,2,1.8,1.7,1.8,1.9,2" }, new string[4] { "4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7,4,6,1,9,3,7", "6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9,6,4,3,7,1,9", "9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7,9,6,3,1,4,7", "7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9,7,4,1,3,6,9" }, 9, new string[3] { "B,C,B,R-R,S,B,S2,R2,B,S-S,C", "B,S,C,B,S-S,B,R2,B,R-R,S2,C", "B,S-S,C,B,S2,R,S,B,C-C,S,R2" }), new RegularGhost(190f, 0.7f, 0.4f), new CircleGhost(0.5f, 85f, 0.3f), new BigGhost(125f, 170f, 0f, 0.1f), new DelayGhost(0f, 0f), new SineGhost(0.6f, 0.6f, 250f, 6f, 3f)));
				break;
			}
			return new Mausoleum(hp, goalTimes, list.ToArray());
		}
	}

	public class Mouse : AbstractLevelProperties<Mouse.State, Mouse.Pattern, Mouse.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Mouse properties { get; private set; }

			public virtual void LevelInit(Mouse properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			BrokenCan = 2,
			Cat = 3
		}

		public enum Pattern
		{
			Move = 0,
			Dash = 1,
			CherryBomb = 2,
			Catapult = 3,
			RomanCandle = 4,
			SawBlades = 5,
			Flame = 6,
			LeftClaw = 7,
			RightClaw = 8,
			GhostMouse = 9,
			Uninitialized = 10
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly CanMove canMove;

			public readonly CanDash canDash;

			public readonly CanCherryBomb canCherryBomb;

			public readonly CanCatapult canCatapult;

			public readonly CanRomanCandle canRomanCandle;

			public readonly BrokenCanSawBlades brokenCanSawBlades;

			public readonly BrokenCanFlame brokenCanFlame;

			public readonly BrokenCanMove brokenCanMove;

			public readonly Claw claw;

			public readonly GhostMouse ghostMouse;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, CanMove canMove, CanDash canDash, CanCherryBomb canCherryBomb, CanCatapult canCatapult, CanRomanCandle canRomanCandle, BrokenCanSawBlades brokenCanSawBlades, BrokenCanFlame brokenCanFlame, BrokenCanMove brokenCanMove, Claw claw, GhostMouse ghostMouse)
				: base(healthTrigger, patterns, stateName)
			{
				this.canMove = canMove;
				this.canDash = canDash;
				this.canCherryBomb = canCherryBomb;
				this.canCatapult = canCatapult;
				this.canRomanCandle = canRomanCandle;
				this.brokenCanSawBlades = brokenCanSawBlades;
				this.brokenCanFlame = brokenCanFlame;
				this.brokenCanMove = brokenCanMove;
				this.claw = claw;
				this.ghostMouse = ghostMouse;
			}
		}

		public class CanMove : AbstractLevelPropertyGroup
		{
			public readonly float speed;

			public readonly MinMax maxXPositionRange;

			public readonly float stopTime;

			public readonly float initialHesitate;

			public CanMove(float speed, MinMax maxXPositionRange, float stopTime, float initialHesitate)
			{
				this.speed = speed;
				this.maxXPositionRange = maxXPositionRange;
				this.stopTime = stopTime;
				this.initialHesitate = initialHesitate;
			}
		}

		public class CanDash : AbstractLevelPropertyGroup
		{
			public readonly float time;

			public readonly float hesitate;

			public readonly MinMax[] springVelocityX;

			public readonly MinMax[] springVelocityY;

			public readonly float springGravity;

			public CanDash(float time, float hesitate, MinMax[] springVelocityX, MinMax[] springVelocityY, float springGravity)
			{
				this.time = time;
				this.hesitate = hesitate;
				this.springVelocityX = springVelocityX;
				this.springVelocityY = springVelocityY;
				this.springGravity = springGravity;
			}
		}

		public class CanCherryBomb : AbstractLevelPropertyGroup
		{
			public readonly string[] patterns;

			public readonly float delay;

			public readonly MinMax xVelocity;

			public readonly MinMax yVelocity;

			public readonly float gravity;

			public readonly int childSpeed;

			public readonly float hesitate;

			public CanCherryBomb(string[] patterns, float delay, MinMax xVelocity, MinMax yVelocity, float gravity, int childSpeed, float hesitate)
			{
				this.patterns = patterns;
				this.delay = delay;
				this.xVelocity = xVelocity;
				this.yVelocity = yVelocity;
				this.gravity = gravity;
				this.childSpeed = childSpeed;
				this.hesitate = hesitate;
			}
		}

		public class CanCatapult : AbstractLevelPropertyGroup
		{
			public readonly string[] patterns;

			public readonly float timeIn;

			public readonly float timeOut;

			public readonly float pumpDelay;

			public readonly float repeatDelay;

			public readonly int projectileSpeed;

			public readonly float angleOffset;

			public readonly float spreadAngle;

			public readonly int count;

			public readonly int hesitate;

			public CanCatapult(string[] patterns, float timeIn, float timeOut, float pumpDelay, float repeatDelay, int projectileSpeed, float angleOffset, float spreadAngle, int count, int hesitate)
			{
				this.patterns = patterns;
				this.timeIn = timeIn;
				this.timeOut = timeOut;
				this.pumpDelay = pumpDelay;
				this.repeatDelay = repeatDelay;
				this.projectileSpeed = projectileSpeed;
				this.angleOffset = angleOffset;
				this.spreadAngle = spreadAngle;
				this.count = count;
				this.hesitate = hesitate;
			}
		}

		public class CanRomanCandle : AbstractLevelPropertyGroup
		{
			public readonly MinMax count;

			public readonly float repeatDelay;

			public readonly float speed;

			public readonly float rotationSpeed;

			public readonly float timeBeforeHoming;

			public readonly float hesitate;

			public CanRomanCandle(MinMax count, float repeatDelay, float speed, float rotationSpeed, float timeBeforeHoming, float hesitate)
			{
				this.count = count;
				this.repeatDelay = repeatDelay;
				this.speed = speed;
				this.rotationSpeed = rotationSpeed;
				this.timeBeforeHoming = timeBeforeHoming;
				this.hesitate = hesitate;
			}
		}

		public class BrokenCanSawBlades : AbstractLevelPropertyGroup
		{
			public readonly string[] patternString;

			public readonly float entrySpeed;

			public readonly float delayBeforeAttack;

			public readonly float delayBeforeNextSaw;

			public readonly float speed;

			public readonly MinMax fullAttackTime;

			public BrokenCanSawBlades(string[] patternString, float entrySpeed, float delayBeforeAttack, float delayBeforeNextSaw, float speed, MinMax fullAttackTime)
			{
				this.patternString = patternString;
				this.entrySpeed = entrySpeed;
				this.delayBeforeAttack = delayBeforeAttack;
				this.delayBeforeNextSaw = delayBeforeNextSaw;
				this.speed = speed;
				this.fullAttackTime = fullAttackTime;
			}
		}

		public class BrokenCanFlame : AbstractLevelPropertyGroup
		{
			public readonly string[] attackString;

			public readonly float delayBeforeShot;

			public readonly float delayAfterShot;

			public readonly float shotSpeed;

			public readonly float chargeTime;

			public readonly float loopTime;

			public BrokenCanFlame(string[] attackString, float delayBeforeShot, float delayAfterShot, float shotSpeed, float chargeTime, float loopTime)
			{
				this.attackString = attackString;
				this.delayBeforeShot = delayBeforeShot;
				this.delayAfterShot = delayAfterShot;
				this.shotSpeed = shotSpeed;
				this.chargeTime = chargeTime;
				this.loopTime = loopTime;
			}
		}

		public class BrokenCanMove : AbstractLevelPropertyGroup
		{
			public readonly float speed;

			public readonly MinMax maxXPositionRange;

			public readonly float stopTime;

			public BrokenCanMove(float speed, MinMax maxXPositionRange, float stopTime)
			{
				this.speed = speed;
				this.maxXPositionRange = maxXPositionRange;
				this.stopTime = stopTime;
			}
		}

		public class Claw : AbstractLevelPropertyGroup
		{
			public readonly float attackDelay;

			public readonly float moveSpeed;

			public readonly float holdGroundTime;

			public readonly float leaveSpeed;

			public readonly string[] fallingObjectStrings;

			public readonly float objectStartingFallSpeed;

			public readonly float objectGravity;

			public readonly float objectSpawnDelay;

			public readonly float hesitateAfterAttack;

			public Claw(float attackDelay, float moveSpeed, float holdGroundTime, float leaveSpeed, string[] fallingObjectStrings, float objectStartingFallSpeed, float objectGravity, float objectSpawnDelay, float hesitateAfterAttack)
			{
				this.attackDelay = attackDelay;
				this.moveSpeed = moveSpeed;
				this.holdGroundTime = holdGroundTime;
				this.leaveSpeed = leaveSpeed;
				this.fallingObjectStrings = fallingObjectStrings;
				this.objectStartingFallSpeed = objectStartingFallSpeed;
				this.objectGravity = objectGravity;
				this.objectSpawnDelay = objectSpawnDelay;
				this.hesitateAfterAttack = hesitateAfterAttack;
			}
		}

		public class GhostMouse : AbstractLevelPropertyGroup
		{
			public readonly bool fourMice;

			public readonly float hp;

			public readonly float jailDuration;

			public readonly MinMax attackDelayRange;

			public readonly float attackAnticipation;

			public readonly float ballSpeed;

			public readonly float splitSpeed;

			public readonly MinMax pinkBallRange;

			public readonly float hesitateAfterAttack;

			public GhostMouse(bool fourMice, float hp, float jailDuration, MinMax attackDelayRange, float attackAnticipation, float ballSpeed, float splitSpeed, MinMax pinkBallRange, float hesitateAfterAttack)
			{
				this.fourMice = fourMice;
				this.hp = hp;
				this.jailDuration = jailDuration;
				this.attackDelayRange = attackDelayRange;
				this.attackAnticipation = attackAnticipation;
				this.ballSpeed = ballSpeed;
				this.splitSpeed = splitSpeed;
				this.pinkBallRange = pinkBallRange;
				this.hesitateAfterAttack = hesitateAfterAttack;
			}
		}

		[CompilerGenerated]
		private static Dictionary<string, int> _003C_003Ef__switch_0024mapA;

		public Mouse(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1500f;
				timeline.events.Add(new Level.Timeline.Event("BrokenCan", 0.5f));
				break;
			case Level.Mode.Normal:
				timeline.health = 2000f;
				timeline.events.Add(new Level.Timeline.Event("BrokenCan", 0.73f));
				timeline.events.Add(new Level.Timeline.Event("Cat", 0.37f));
				break;
			case Level.Mode.Hard:
				timeline.health = 2100f;
				timeline.events.Add(new Level.Timeline.Event("BrokenCan", 0.73f));
				timeline.events.Add(new Level.Timeline.Event("Cat", 0.37f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null)
			{
				if (_003C_003Ef__switch_0024mapA == null)
				{
					Dictionary<string, int> dictionary = new Dictionary<string, int>(10);
					dictionary.Add("M", 0);
					dictionary.Add("D", 1);
					dictionary.Add("B", 2);
					dictionary.Add("C", 3);
					dictionary.Add("R", 4);
					dictionary.Add("S", 5);
					dictionary.Add("F", 6);
					dictionary.Add("L", 7);
					dictionary.Add("X", 8);
					dictionary.Add("G", 9);
					_003C_003Ef__switch_0024mapA = dictionary;
				}
				int value;
				if (_003C_003Ef__switch_0024mapA.TryGetValue(id, out value))
				{
					switch (value)
					{
					case 0:
						return Pattern.Move;
					case 1:
						return Pattern.Dash;
					case 2:
						return Pattern.CherryBomb;
					case 3:
						return Pattern.Catapult;
					case 4:
						return Pattern.RomanCandle;
					case 5:
						return Pattern.SawBlades;
					case 6:
						return Pattern.Flame;
					case 7:
						return Pattern.LeftClaw;
					case 8:
						return Pattern.RightClaw;
					case 9:
						return Pattern.GhostMouse;
					}
				}
			}
			Debug.LogError("Pattern Mouse.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static Mouse GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1500;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[8]
				{
					Pattern.Dash,
					Pattern.Catapult,
					Pattern.CherryBomb,
					Pattern.Dash,
					Pattern.CherryBomb,
					Pattern.Catapult,
					Pattern.Dash,
					Pattern.Catapult
				} }, States.Main, new CanMove(230f, new MinMax(250f, 400f), 0f, 1f), new CanDash(1.35f, 1f, new MinMax[2]
				{
					new MinMax(-280f, -330f),
					new MinMax(-400f, -440f)
				}, new MinMax[2]
				{
					new MinMax(600f, 650f),
					new MinMax(750f, 790f)
				}, 900f), new CanCherryBomb(new string[3] { "D:1,P:3", "D:1,P:2,D:0.5,P:1", "D:0.5,P:1,D:0.5,P:2" }, 1.2f, new MinMax(-210f, -400f), new MinMax(400f, 650f), 1000f, 800, 1f), new CanCatapult(new string[4] { "CGGGN", "GGGNC", "CGGNG", "BGGCG" }, 0.7f, 0.7f, 1f, 1f, 640, 15f, 90f, 2, 1), new CanRomanCandle(new MinMax(1f, 1f), 1f, 550f, 4f, 1f, 1f), new BrokenCanSawBlades(new string[0], 50f, 1f, 1f, 300f, new MinMax(10000f, 10005f)), new BrokenCanFlame(new string[1] { "F" }, 1f, 1.5f, 400f, 1f, 0.75f), new BrokenCanMove(235f, new MinMax(300f, 400f), 0.25f), new Claw(0f, 0f, 0f, 0f, new string[0], 0f, 0f, 0f, 0f), new GhostMouse(false, 0f, 0f, new MinMax(0f, 1f), 0f, 0f, 0f, new MinMax(0f, 1f), 0f)));
				list.Add(new State(0.5f, new Pattern[1][] { new Pattern[1] { Pattern.Flame } }, States.BrokenCan, new CanMove(230f, new MinMax(250f, 400f), 0f, 1f), new CanDash(1.35f, 1f, new MinMax[2]
				{
					new MinMax(-280f, -330f),
					new MinMax(-400f, -440f)
				}, new MinMax[2]
				{
					new MinMax(600f, 650f),
					new MinMax(750f, 790f)
				}, 900f), new CanCherryBomb(new string[3] { "D:1,P:3", "D:1,P:2,D:0.5,P:1", "D:0.5,P:1,D:0.5,P:2" }, 1.2f, new MinMax(-210f, -400f), new MinMax(400f, 650f), 1000f, 800, 1f), new CanCatapult(new string[4] { "CGGGN", "GGGNC", "CGGNG", "BGGCG" }, 0.7f, 0.7f, 1f, 1f, 640, 15f, 90f, 2, 1), new CanRomanCandle(new MinMax(1f, 1f), 1f, 550f, 4f, 1f, 1f), new BrokenCanSawBlades(new string[0], 50f, 1f, 1f, 300f, new MinMax(10000f, 10005f)), new BrokenCanFlame(new string[1] { "F" }, 1f, 1.5f, 400f, 1f, 0.75f), new BrokenCanMove(235f, new MinMax(300f, 400f), 0.25f), new Claw(0f, 0f, 0f, 0f, new string[0], 0f, 0f, 0f, 0f), new GhostMouse(false, 0f, 0f, new MinMax(0f, 1f), 0f, 0f, 0f, new MinMax(0f, 1f), 0f)));
				break;
			case Level.Mode.Normal:
				hp = 2000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[14]
				{
					Pattern.CherryBomb,
					Pattern.Dash,
					Pattern.Catapult,
					Pattern.Dash,
					Pattern.CherryBomb,
					Pattern.Catapult,
					Pattern.Dash,
					Pattern.CherryBomb,
					Pattern.Catapult,
					Pattern.CherryBomb,
					Pattern.Dash,
					Pattern.CherryBomb,
					Pattern.Catapult,
					Pattern.Dash
				} }, States.Main, new CanMove(280f, new MinMax(230f, 425f), 0f, 1f), new CanDash(1.1f, 0.5f, new MinMax[2]
				{
					new MinMax(-280f, -330f),
					new MinMax(-400f, -440f)
				}, new MinMax[2]
				{
					new MinMax(600f, 650f),
					new MinMax(750f, 790f)
				}, 900f), new CanCherryBomb(new string[4] { "D:0.4, P:2,D:1.3,P:2,D:0.6,P:2", "D:0.5,P:3,D:0.7,P:2", "D:0.4, P:2,D:0.6,P:2,D:1.3,P:2", "D:0.5,P:2,D:0.7,P:3" }, 0.8f, new MinMax(-150f, -450f), new MinMax(400f, 700f), 1000f, 850, 1f), new CanCatapult(new string[4] { "BNGCG", "CGPGC", "PGGCN", "BPGGN" }, 0.7f, 0.7f, 1f, 1f, 670, 15f, 75f, 2, 1), new CanRomanCandle(new MinMax(2f, 4f), 1f, 600f, 4f, 1f, 1f), new BrokenCanSawBlades(new string[5] { "1,6,3,2,4,5", "1,2,5,3,6,4", "3,5,1,4,6,2", "5,1,3,6,4,2", "6,3,1,5,2,4" }, 45f, 2.2f, 2.6f, 280f, new MinMax(10f, 15f)), new BrokenCanFlame(new string[1] { "F" }, 1f, 1.5f, 400f, 1.3f, 0.75f), new BrokenCanMove(100f, new MinMax(20f, 50f), 0.25f), new Claw(1f, 1500f, 0.083f, 500f, new string[2] { "50,250,450,650,850,1050,1250", "1200,1000,800,600,400,200,0" }, 30f, 740f, 0.65f, 1f), new GhostMouse(false, 34f, 2f, new MinMax(2.5f, 4f), 0.5f, 630f, 800f, new MinMax(1f, 2f), 1f)));
				list.Add(new State(0.73f, new Pattern[1][] { new Pattern[1] { Pattern.Flame } }, States.BrokenCan, new CanMove(280f, new MinMax(230f, 425f), 0f, 1f), new CanDash(1.1f, 0.5f, new MinMax[2]
				{
					new MinMax(-280f, -330f),
					new MinMax(-400f, -440f)
				}, new MinMax[2]
				{
					new MinMax(600f, 650f),
					new MinMax(750f, 790f)
				}, 900f), new CanCherryBomb(new string[4] { "D:0.4, P:2,D:1.3,P:2,D:0.6,P:2", "D:0.5,P:3,D:0.7,P:2", "D:0.4, P:2,D:0.6,P:2,D:1.3,P:2", "D:0.5,P:2,D:0.7,P:3" }, 0.8f, new MinMax(-150f, -450f), new MinMax(400f, 700f), 1000f, 850, 1f), new CanCatapult(new string[4] { "BNGCG", "CGPGC", "PGGCN", "BPGGN" }, 0.7f, 0.7f, 1f, 1f, 670, 15f, 75f, 2, 1), new CanRomanCandle(new MinMax(2f, 4f), 1f, 600f, 4f, 1f, 1f), new BrokenCanSawBlades(new string[5] { "1,6,3,2,4,5", "1,2,5,3,6,4", "3,5,1,4,6,2", "5,1,3,6,4,2", "6,3,1,5,2,4" }, 45f, 2.2f, 2.6f, 280f, new MinMax(10f, 15f)), new BrokenCanFlame(new string[1] { "F" }, 1f, 1.5f, 400f, 1.3f, 0.75f), new BrokenCanMove(100f, new MinMax(20f, 50f), 0.25f), new Claw(1f, 1500f, 0.083f, 500f, new string[2] { "50,250,450,650,850,1050,1250", "1200,1000,800,600,400,200,0" }, 30f, 740f, 0.65f, 1f), new GhostMouse(false, 34f, 2f, new MinMax(2.5f, 4f), 0.5f, 630f, 800f, new MinMax(1f, 2f), 1f)));
				list.Add(new State(0.37f, new Pattern[1][] { new Pattern[9]
				{
					Pattern.LeftClaw,
					Pattern.RightClaw,
					Pattern.GhostMouse,
					Pattern.RightClaw,
					Pattern.LeftClaw,
					Pattern.GhostMouse,
					Pattern.LeftClaw,
					Pattern.RightClaw,
					Pattern.GhostMouse
				} }, States.Cat, new CanMove(280f, new MinMax(230f, 425f), 0f, 1f), new CanDash(1.1f, 0.5f, new MinMax[2]
				{
					new MinMax(-280f, -330f),
					new MinMax(-400f, -440f)
				}, new MinMax[2]
				{
					new MinMax(600f, 650f),
					new MinMax(750f, 790f)
				}, 900f), new CanCherryBomb(new string[4] { "D:0.4, P:2,D:1.3,P:2,D:0.6,P:2", "D:0.5,P:3,D:0.7,P:2", "D:0.4, P:2,D:0.6,P:2,D:1.3,P:2", "D:0.5,P:2,D:0.7,P:3" }, 0.8f, new MinMax(-150f, -450f), new MinMax(400f, 700f), 1000f, 850, 1f), new CanCatapult(new string[4] { "BNGCG", "CGPGC", "PGGCN", "BPGGN" }, 0.7f, 0.7f, 1f, 1f, 670, 15f, 75f, 2, 1), new CanRomanCandle(new MinMax(2f, 4f), 1f, 600f, 4f, 1f, 1f), new BrokenCanSawBlades(new string[5] { "1,6,3,2,4,5", "1,2,5,3,6,4", "3,5,1,4,6,2", "5,1,3,6,4,2", "6,3,1,5,2,4" }, 45f, 2.2f, 2.6f, 280f, new MinMax(10f, 15f)), new BrokenCanFlame(new string[1] { "F" }, 1f, 1.5f, 400f, 1.3f, 0.75f), new BrokenCanMove(100f, new MinMax(20f, 50f), 0.25f), new Claw(1f, 1500f, 0.083f, 500f, new string[2] { "50,250,450,650,850,1050,1250", "1200,1000,800,600,400,200,0" }, 30f, 740f, 0.65f, 1f), new GhostMouse(false, 34f, 2f, new MinMax(2.5f, 4f), 0.5f, 630f, 800f, new MinMax(1f, 2f), 1f)));
				break;
			case Level.Mode.Hard:
				hp = 2100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[10]
				{
					Pattern.CherryBomb,
					Pattern.Catapult,
					Pattern.Dash,
					Pattern.CherryBomb,
					Pattern.Dash,
					Pattern.Catapult,
					Pattern.Dash,
					Pattern.Catapult,
					Pattern.CherryBomb,
					Pattern.Dash
				} }, States.Main, new CanMove(300f, new MinMax(250f, 450f), 0f, 1f), new CanDash(0.95f, 0.5f, new MinMax[2]
				{
					new MinMax(-280f, -330f),
					new MinMax(-400f, -440f)
				}, new MinMax[2]
				{
					new MinMax(600f, 650f),
					new MinMax(750f, 790f)
				}, 900f), new CanCherryBomb(new string[6] { "D:0.4, P:3,D:1.3,P:3", "D:0.4,P:4,D:1,P:2", "D:0.4,P:2,D:0.7,P:2,D:0.4,P:2", "D:0.4,P:3,D:1.5,P:3", "D:0.4,P:2,D:1,P:4", "D:0.4,P:2,D:0.4,P:2,D0.7,P:2" }, 0.7f, new MinMax(-200f, -450f), new MinMax(430f, 700f), 1150f, 1000, 0.7f), new CanCatapult(new string[5] { "BNGCG", "CBGGN", "NGGCB", "CGBGN", "NBGGC" }, 0.7f, 0.7f, 1f, 1f, 770, 15f, 65f, 2, 1), new CanRomanCandle(new MinMax(2f, 4f), 1f, 640f, 4f, 0.8f, 1f), new BrokenCanSawBlades(new string[6] { "1,6,3,5,2,4", "3,1,4,6,2,5", "1,2,5,3,6,4", "2,5,3,6,1,4", "6,2,5,1,4,3", "6,1,3,5,2,4" }, 50f, 1.7f, 1.8f, 300f, new MinMax(7f, 11f)), new BrokenCanFlame(new string[1] { "F" }, 1f, 1.5f, 400f, 1.3f, 0.75f), new BrokenCanMove(100f, new MinMax(20f, 50f), 0.25f), new Claw(0.65f, 1500f, 0.083f, 500f, new string[2] { "50,250,450,650,850,1050,1250", "1200,1000,800,600,400,200,0" }, 30f, 800f, 0.6f, 1f), new GhostMouse(true, 30f, 2f, new MinMax(2f, 3.5f), 0.5f, 720f, 1000f, new MinMax(2f, 2f), 1f)));
				list.Add(new State(0.73f, new Pattern[1][] { new Pattern[1] { Pattern.Flame } }, States.BrokenCan, new CanMove(300f, new MinMax(250f, 450f), 0f, 1f), new CanDash(0.95f, 0.5f, new MinMax[2]
				{
					new MinMax(-280f, -330f),
					new MinMax(-400f, -440f)
				}, new MinMax[2]
				{
					new MinMax(600f, 650f),
					new MinMax(750f, 790f)
				}, 900f), new CanCherryBomb(new string[6] { "D:0.4, P:3,D:1.3,P:3", "D:0.4,P:4,D:1,P:2", "D:0.4,P:2,D:0.7,P:2,D:0.4,P:2", "D:0.4,P:3,D:1.5,P:3", "D:0.4,P:2,D:1,P:4", "D:0.4,P:2,D:0.4,P:2,D0.7,P:2" }, 0.7f, new MinMax(-200f, -450f), new MinMax(430f, 700f), 1150f, 1000, 0.7f), new CanCatapult(new string[5] { "BNGCG", "CBGGN", "NGGCB", "CGBGN", "NBGGC" }, 0.7f, 0.7f, 1f, 1f, 770, 15f, 65f, 2, 1), new CanRomanCandle(new MinMax(2f, 4f), 1f, 640f, 4f, 0.8f, 1f), new BrokenCanSawBlades(new string[6] { "1,6,3,5,2,4", "3,1,4,6,2,5", "1,2,5,3,6,4", "2,5,3,6,1,4", "6,2,5,1,4,3", "6,1,3,5,2,4" }, 50f, 1.7f, 1.8f, 300f, new MinMax(7f, 11f)), new BrokenCanFlame(new string[1] { "F" }, 1f, 1.5f, 400f, 1.3f, 0.75f), new BrokenCanMove(100f, new MinMax(20f, 50f), 0.25f), new Claw(0.65f, 1500f, 0.083f, 500f, new string[2] { "50,250,450,650,850,1050,1250", "1200,1000,800,600,400,200,0" }, 30f, 800f, 0.6f, 1f), new GhostMouse(true, 30f, 2f, new MinMax(2f, 3.5f), 0.5f, 720f, 1000f, new MinMax(2f, 2f), 1f)));
				list.Add(new State(0.37f, new Pattern[1][] { new Pattern[9]
				{
					Pattern.RightClaw,
					Pattern.LeftClaw,
					Pattern.GhostMouse,
					Pattern.LeftClaw,
					Pattern.RightClaw,
					Pattern.GhostMouse,
					Pattern.RightClaw,
					Pattern.LeftClaw,
					Pattern.GhostMouse
				} }, States.Cat, new CanMove(300f, new MinMax(250f, 450f), 0f, 1f), new CanDash(0.95f, 0.5f, new MinMax[2]
				{
					new MinMax(-280f, -330f),
					new MinMax(-400f, -440f)
				}, new MinMax[2]
				{
					new MinMax(600f, 650f),
					new MinMax(750f, 790f)
				}, 900f), new CanCherryBomb(new string[6] { "D:0.4, P:3,D:1.3,P:3", "D:0.4,P:4,D:1,P:2", "D:0.4,P:2,D:0.7,P:2,D:0.4,P:2", "D:0.4,P:3,D:1.5,P:3", "D:0.4,P:2,D:1,P:4", "D:0.4,P:2,D:0.4,P:2,D0.7,P:2" }, 0.7f, new MinMax(-200f, -450f), new MinMax(430f, 700f), 1150f, 1000, 0.7f), new CanCatapult(new string[5] { "BNGCG", "CBGGN", "NGGCB", "CGBGN", "NBGGC" }, 0.7f, 0.7f, 1f, 1f, 770, 15f, 65f, 2, 1), new CanRomanCandle(new MinMax(2f, 4f), 1f, 640f, 4f, 0.8f, 1f), new BrokenCanSawBlades(new string[6] { "1,6,3,5,2,4", "3,1,4,6,2,5", "1,2,5,3,6,4", "2,5,3,6,1,4", "6,2,5,1,4,3", "6,1,3,5,2,4" }, 50f, 1.7f, 1.8f, 300f, new MinMax(7f, 11f)), new BrokenCanFlame(new string[1] { "F" }, 1f, 1.5f, 400f, 1.3f, 0.75f), new BrokenCanMove(100f, new MinMax(20f, 50f), 0.25f), new Claw(0.65f, 1500f, 0.083f, 500f, new string[2] { "50,250,450,650,850,1050,1250", "1200,1000,800,600,400,200,0" }, 30f, 800f, 0.6f, 1f), new GhostMouse(true, 30f, 2f, new MinMax(2f, 3.5f), 0.5f, 720f, 1000f, new MinMax(2f, 2f), 1f)));
				break;
			}
			return new Mouse(hp, goalTimes, list.ToArray());
		}
	}

	public class Pirate : AbstractLevelProperties<Pirate.State, Pirate.Pattern, Pirate.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Pirate properties { get; private set; }

			public virtual void LevelInit(Pirate properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			Boat = 2
		}

		public enum Pattern
		{
			Shark = 0,
			Squid = 1,
			DogFish = 2,
			Peashot = 3,
			Boat = 4,
			Uninitialized = 5
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Squid squid;

			public readonly Shark shark;

			public readonly DogFish dogFish;

			public readonly Peashot peashot;

			public readonly Barrel barrel;

			public readonly Cannon cannon;

			public readonly Boat boat;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Squid squid, Shark shark, DogFish dogFish, Peashot peashot, Barrel barrel, Cannon cannon, Boat boat)
				: base(healthTrigger, patterns, stateName)
			{
				this.squid = squid;
				this.shark = shark;
				this.dogFish = dogFish;
				this.peashot = peashot;
				this.barrel = barrel;
				this.cannon = cannon;
				this.boat = boat;
			}
		}

		public class Squid : AbstractLevelPropertyGroup
		{
			public readonly float startDelay;

			public readonly int endDelay;

			public readonly MinMax hp;

			public readonly float maxTime;

			public readonly MinMax xPos;

			public readonly float opacityAdd;

			public readonly float opacityAddTime;

			public readonly float darkHoldTime;

			public readonly float darkFadeTime;

			public readonly float blobDelay;

			public readonly float blobGravity;

			public readonly MinMax blobVelX;

			public readonly MinMax blobVelY;

			public Squid(float startDelay, int endDelay, MinMax hp, float maxTime, MinMax xPos, float opacityAdd, float opacityAddTime, float darkHoldTime, float darkFadeTime, float blobDelay, float blobGravity, MinMax blobVelX, MinMax blobVelY)
			{
				this.startDelay = startDelay;
				this.endDelay = endDelay;
				this.hp = hp;
				this.maxTime = maxTime;
				this.xPos = xPos;
				this.opacityAdd = opacityAdd;
				this.opacityAddTime = opacityAddTime;
				this.darkHoldTime = darkHoldTime;
				this.darkFadeTime = darkFadeTime;
				this.blobDelay = blobDelay;
				this.blobGravity = blobGravity;
				this.blobVelX = blobVelX;
				this.blobVelY = blobVelY;
			}
		}

		public class Shark : AbstractLevelPropertyGroup
		{
			public readonly float startDelay;

			public readonly float endDelay;

			public readonly float finTime;

			public readonly float exitSpeed;

			public readonly float shotExitSpeed;

			public readonly float attackDelay;

			public readonly float x;

			public Shark(float startDelay, float endDelay, float finTime, float exitSpeed, float shotExitSpeed, float attackDelay, float x)
			{
				this.startDelay = startDelay;
				this.endDelay = endDelay;
				this.finTime = finTime;
				this.exitSpeed = exitSpeed;
				this.shotExitSpeed = shotExitSpeed;
				this.attackDelay = attackDelay;
				this.x = x;
			}
		}

		public class DogFish : AbstractLevelPropertyGroup
		{
			public readonly float startDelay;

			public readonly float endDelay;

			public readonly float startSpeed;

			public readonly float endSpeed;

			public readonly float speedFalloffTime;

			public readonly int hp;

			public readonly int count;

			public readonly MinMax nextFishDelay;

			public readonly float deathSpeed;

			public DogFish(float startDelay, float endDelay, float startSpeed, float endSpeed, float speedFalloffTime, int hp, int count, MinMax nextFishDelay, float deathSpeed)
			{
				this.startDelay = startDelay;
				this.endDelay = endDelay;
				this.startSpeed = startSpeed;
				this.endSpeed = endSpeed;
				this.speedFalloffTime = speedFalloffTime;
				this.hp = hp;
				this.count = count;
				this.nextFishDelay = nextFishDelay;
				this.deathSpeed = deathSpeed;
			}
		}

		public class Peashot : AbstractLevelPropertyGroup
		{
			public readonly float startDelay;

			public readonly int endDelay;

			public readonly string[] patterns;

			public readonly int damage;

			public readonly float speed;

			public readonly float shotDelay;

			public readonly string shotType;

			public Peashot(float startDelay, int endDelay, string[] patterns, int damage, float speed, float shotDelay, string shotType)
			{
				this.startDelay = startDelay;
				this.endDelay = endDelay;
				this.patterns = patterns;
				this.damage = damage;
				this.speed = speed;
				this.shotDelay = shotDelay;
				this.shotType = shotType;
			}
		}

		public class Barrel : AbstractLevelPropertyGroup
		{
			public readonly float damage;

			public readonly float moveTime;

			public readonly float fallTime;

			public readonly float riseTime;

			public readonly float safeTime;

			public readonly float groundHold;

			public Barrel(float damage, float moveTime, float fallTime, float riseTime, float safeTime, float groundHold)
			{
				this.damage = damage;
				this.moveTime = moveTime;
				this.fallTime = fallTime;
				this.riseTime = riseTime;
				this.safeTime = safeTime;
				this.groundHold = groundHold;
			}
		}

		public class Cannon : AbstractLevelPropertyGroup
		{
			public readonly bool firing;

			public readonly float damage;

			public readonly float speed;

			public readonly MinMax delayRange;

			public Cannon(bool firing, float damage, float speed, MinMax delayRange)
			{
				this.firing = firing;
				this.damage = damage;
				this.speed = speed;
				this.delayRange = delayRange;
			}
		}

		public class Boat : AbstractLevelPropertyGroup
		{
			public readonly float pirateFallDelay;

			public readonly float pirateFallTime;

			public readonly float winceDuration;

			public readonly float attackDelay;

			public readonly float bulletSpeed;

			public readonly float bulletRotationSpeed;

			public readonly float bulletDelay;

			public readonly int bulletCount;

			public readonly float bulletPostWait;

			public readonly float beamDelay;

			public readonly float beamDuration;

			public readonly float beamPostWait;

			public Boat(float pirateFallDelay, float pirateFallTime, float winceDuration, float attackDelay, float bulletSpeed, float bulletRotationSpeed, float bulletDelay, int bulletCount, float bulletPostWait, float beamDelay, float beamDuration, float beamPostWait)
			{
				this.pirateFallDelay = pirateFallDelay;
				this.pirateFallTime = pirateFallTime;
				this.winceDuration = winceDuration;
				this.attackDelay = attackDelay;
				this.bulletSpeed = bulletSpeed;
				this.bulletRotationSpeed = bulletRotationSpeed;
				this.bulletDelay = bulletDelay;
				this.bulletCount = bulletCount;
				this.bulletPostWait = bulletPostWait;
				this.beamDelay = beamDelay;
				this.beamDuration = beamDuration;
				this.beamPostWait = beamPostWait;
			}
		}

		public Pirate(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1000f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.99f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.82f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.68f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.52f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.38f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.22f));
				break;
			case Level.Mode.Normal:
				timeline.health = 1200f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.87f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.51f));
				timeline.events.Add(new Level.Timeline.Event("Boat", 0.22f));
				break;
			case Level.Mode.Hard:
				timeline.health = 1400f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.92f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.77f));
				timeline.events.Add(new Level.Timeline.Event("Boat", 0.32f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "S":
				return Pattern.Shark;
			case "Q":
				return Pattern.Squid;
			case "F":
				return Pattern.DogFish;
			case "P":
				return Pattern.Peashot;
			case "B":
				return Pattern.Boat;
			default:
				Debug.LogError("Pattern Pirate.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static Pirate GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main, new Squid(2f, 2, new MinMax(41f, 61f), 7.5f, new MinMax(-30f, -120f), 0.4f, 0.4f, 2.8f, 5f, 0.21f, 900f, new MinMax(-330f, 330f), new MinMax(550f, 700f)), new Shark(1f, 2f, 2f, 210f, 300f, 1.5f, -50f), new DogFish(1f, 2f, 650f, 550f, 2f, 3, 2, new MinMax(0.3f, 0.5f), 400f), new Peashot(1f, 3, new string[3] { "D:0.5, P:1, D:1, P:2", "D:1.5,P:2,D:1,P:1", "D:1,P:3" }, 1, 490f, 0.9f, "P,P,R"), new Barrel(1f, 3.4f, 1.2f, 1.3f, 3.5f, 1.2f), new Cannon(true, 1f, 725f, new MinMax(1.8f, 2.6f)), new Boat(3f, 0.7f, 2f, 2f, 300f, 360f, 0.7f, 3, 2f, 1f, 3f, 2f)));
				list.Add(new State(0.99f, new Pattern[1][] { new Pattern[0] }, States.Generic, new Squid(2f, 2, new MinMax(41f, 61f), 7.5f, new MinMax(-30f, -120f), 0.4f, 0.4f, 2.8f, 5f, 0.21f, 900f, new MinMax(-330f, 330f), new MinMax(550f, 700f)), new Shark(1f, 2f, 2f, 210f, 300f, 1.5f, -50f), new DogFish(1f, 2f, 650f, 550f, 2f, 3, 2, new MinMax(0.3f, 0.5f), 400f), new Peashot(1f, 3, new string[3] { "D:0.5, P:1, D:1, P:2", "D:1.5,P:2,D:1,P:1", "D:1,P:3" }, 1, 490f, 0.9f, "P,P,R"), new Barrel(1f, 3.4f, 1.2f, 1.3f, 3.5f, 1.2f), new Cannon(true, 1f, 725f, new MinMax(1.8f, 2.6f)), new Boat(3f, 0.7f, 2f, 2f, 300f, 360f, 0.7f, 3, 2f, 1f, 3f, 2f)));
				list.Add(new State(0.82f, new Pattern[1][] { new Pattern[1] { Pattern.Peashot } }, States.Generic, new Squid(2f, 2, new MinMax(41f, 61f), 7.5f, new MinMax(-30f, -120f), 0.4f, 0.4f, 2.8f, 5f, 0.21f, 900f, new MinMax(-330f, 330f), new MinMax(550f, 700f)), new Shark(1f, 2f, 2f, 210f, 300f, 1.5f, -50f), new DogFish(1f, 2f, 650f, 550f, 2f, 3, 2, new MinMax(0.3f, 0.5f), 400f), new Peashot(1f, 3, new string[3] { "D:0.5, P:1, D:1, P:2", "D:1.5,P:2,D:1,P:1", "D:1,P:3" }, 1, 490f, 0.9f, "P,P,R"), new Barrel(1f, 3.4f, 1.2f, 1.3f, 3.5f, 1.2f), new Cannon(false, 0f, 0f, new MinMax(0f, 1f)), new Boat(3f, 0.7f, 2f, 2f, 300f, 360f, 0.7f, 3, 2f, 1f, 3f, 2f)));
				list.Add(new State(0.68f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Shark,
					Pattern.Peashot
				} }, States.Generic, new Squid(2f, 2, new MinMax(41f, 61f), 7.5f, new MinMax(-30f, -120f), 0.4f, 0.4f, 2.8f, 5f, 0.21f, 900f, new MinMax(-330f, 330f), new MinMax(550f, 700f)), new Shark(1f, 2f, 2f, 210f, 300f, 1.5f, -50f), new DogFish(1f, 2f, 650f, 550f, 2f, 3, 2, new MinMax(0.3f, 0.5f), 400f), new Peashot(1f, 3, new string[6] { "D:1,P1,D:0.5,P:1", "D:0.5,P:1,D:1,P:1", "D:1,P:2", "D:0.5,P:1,D:1.5,P:1", "D:0.5,P:1,D:0.5,P:1", "D:1,P:2" }, 1, 490f, 0.9f, "P,R"), new Barrel(1f, 3.4f, 1.2f, 1.3f, 3.5f, 1.2f), new Cannon(false, 0f, 0f, new MinMax(0f, 1f)), new Boat(3f, 0.7f, 2f, 2f, 300f, 360f, 0.7f, 3, 2f, 1f, 3f, 2f)));
				list.Add(new State(0.52f, new Pattern[1][] { new Pattern[1] { Pattern.DogFish } }, States.Generic, new Squid(2f, 2, new MinMax(41f, 61f), 7.5f, new MinMax(-30f, -120f), 0.4f, 0.4f, 2.8f, 5f, 0.21f, 900f, new MinMax(-330f, 330f), new MinMax(550f, 700f)), new Shark(1f, 2f, 2f, 210f, 300f, 1.5f, -50f), new DogFish(1f, 2f, 650f, 550f, 2f, 3, 2, new MinMax(0.3f, 0.5f), 400f), new Peashot(1f, 3, new string[6] { "D:1,P1,D:0.5,P:1", "D:0.5,P:1,D:1,P:1", "D:1,P:2", "D:0.5,P:1,D:1.5,P:1", "D:0.5,P:1,D:0.5,P:1", "D:1,P:2" }, 1, 490f, 0.9f, "P,R"), new Barrel(1f, 3.4f, 1.2f, 1.3f, 3.5f, 1.2f), new Cannon(false, 1f, 0f, new MinMax(0f, 1f)), new Boat(3f, 0.7f, 2f, 2f, 300f, 360f, 0.7f, 3, 2f, 1f, 3f, 2f)));
				list.Add(new State(0.38f, new Pattern[1][] { new Pattern[1] { Pattern.Squid } }, States.Generic, new Squid(2f, 2, new MinMax(41f, 61f), 7.5f, new MinMax(-30f, -120f), 0.4f, 0.4f, 2.8f, 5f, 0.21f, 900f, new MinMax(-330f, 330f), new MinMax(550f, 700f)), new Shark(1f, 2f, 2f, 210f, 300f, 1.5f, -50f), new DogFish(1f, 2f, 650f, 550f, 2f, 3, 2, new MinMax(0.3f, 0.5f), 400f), new Peashot(1f, 3, new string[6] { "D:1,P1,D:0.5,P:1", "D:0.5,P:1,D:1,P:1", "D:1,P:2", "D:0.5,P:1,D:1.5,P:1", "D:0.5,P:1,D:0.5,P:1", "D:1,P:2" }, 1, 490f, 0.9f, "P,R"), new Barrel(1f, 3.4f, 1.2f, 1.3f, 3.5f, 1.2f), new Cannon(true, 1f, 735f, new MinMax(2.8f, 4.3f)), new Boat(3f, 0.7f, 2f, 2f, 300f, 360f, 0.7f, 3, 2f, 1f, 3f, 2f)));
				list.Add(new State(0.22f, new Pattern[1][] { new Pattern[14]
				{
					Pattern.Peashot,
					Pattern.Peashot,
					Pattern.Peashot,
					Pattern.Shark,
					Pattern.Peashot,
					Pattern.Peashot,
					Pattern.Shark,
					Pattern.Peashot,
					Pattern.Peashot,
					Pattern.Shark,
					Pattern.Peashot,
					Pattern.Peashot,
					Pattern.Peashot,
					Pattern.Shark
				} }, States.Generic, new Squid(2f, 2, new MinMax(41f, 61f), 7.5f, new MinMax(-30f, -120f), 0.4f, 0.4f, 2.8f, 5f, 0.21f, 900f, new MinMax(-330f, 330f), new MinMax(550f, 700f)), new Shark(1f, 2f, 2f, 210f, 300f, 1.5f, -50f), new DogFish(1f, 2f, 650f, 550f, 2f, 3, 2, new MinMax(0.3f, 0.5f), 400f), new Peashot(1f, 3, new string[8] { "P:2,D:1.5,P:1,D:1.5,P:2", "P:2,D:2,P:2,D:1.5,P:1", "D:1,P:3", "P:1,D:1,P:2,D:2,P:2", "D:0.5,P:1,D:2,P:3", "P:2,D:2,P:2,D:1.5,P:1", "P:2,D:1,P:1,D:2,P:2", "D:1.5,P:4" }, 1, 500f, 0.9f, "P,P,P,R"), new Barrel(1f, 3.4f, 1.2f, 1.3f, 3.5f, 1.2f), new Cannon(true, 1f, 745f, new MinMax(3.8f, 4.8f)), new Boat(3f, 0.7f, 2f, 2f, 300f, 360f, 0.7f, 3, 2f, 1f, 3f, 2f)));
				break;
			case Level.Mode.Normal:
				hp = 1200;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] { Pattern.Peashot } }, States.Main, new Squid(2f, 1, new MinMax(31f, 61f), 5.5f, new MinMax(-200f, -60f), 0.4f, 0.4f, 3.3f, 5f, 0.12f, 1000f, new MinMax(-300f, 300f), new MinMax(500f, 800f)), new Shark(1f, 1f, 2f, 240f, 310f, 1.5f, 50f), new DogFish(2f, 1f, 700f, 500f, 2f, 3, 4, new MinMax(1.3f, 2f), 400f), new Peashot(2f, 3, new string[3] { "D:1, P:3, D:1, P:1", "D:1, P:2, D:1.5, P:2", "D:1, P:1, D:1, P:3" }, 1, 550f, 0.65f, "P,P,P,R"), new Barrel(1f, 3.6f, 0.9f, 1f, 3f, 0.8f), new Cannon(false, 1f, 850f, new MinMax(3.4f, 4f)), new Boat(0f, 200f, 1f, 2f, 0f, 0f, 0f, 0, 0f, 0f, 0f, 0f)));
				list.Add(new State(0.87f, new Pattern[2][]
				{
					new Pattern[6]
					{
						Pattern.Peashot,
						Pattern.Shark,
						Pattern.Peashot,
						Pattern.Squid,
						Pattern.Peashot,
						Pattern.DogFish
					},
					new Pattern[6]
					{
						Pattern.Squid,
						Pattern.Peashot,
						Pattern.Shark,
						Pattern.Peashot,
						Pattern.DogFish,
						Pattern.Peashot
					}
				}, States.Generic, new Squid(2f, 1, new MinMax(31f, 61f), 5.5f, new MinMax(-200f, -60f), 0.4f, 0.4f, 3.3f, 5f, 0.12f, 1000f, new MinMax(-300f, 300f), new MinMax(500f, 800f)), new Shark(1f, 1f, 2f, 240f, 310f, 1.5f, 50f), new DogFish(2f, 1f, 700f, 500f, 2f, 3, 4, new MinMax(1.3f, 2f), 400f), new Peashot(1f, 3, new string[2] { "D:1, P:1, D:.5, P:1, D:2, P:2", "D:1. P:2. D:2, P:1, D:.5, P:1" }, 1, 550f, 0.65f, "P,P,P,R"), new Barrel(1f, 3.6f, 0.9f, 1f, 3f, 0.8f), new Cannon(false, 1f, 850f, new MinMax(3.4f, 4f)), new Boat(0f, 200f, 1f, 2f, 0f, 0f, 0f, 0, 0f, 0f, 0f, 0f)));
				list.Add(new State(0.51f, new Pattern[2][]
				{
					new Pattern[6]
					{
						Pattern.Peashot,
						Pattern.Shark,
						Pattern.Peashot,
						Pattern.Squid,
						Pattern.Peashot,
						Pattern.DogFish
					},
					new Pattern[6]
					{
						Pattern.DogFish,
						Pattern.Peashot,
						Pattern.Squid,
						Pattern.Peashot,
						Pattern.Shark,
						Pattern.Peashot
					}
				}, States.Generic, new Squid(2f, 1, new MinMax(31f, 61f), 5.5f, new MinMax(-200f, -60f), 0.4f, 0.4f, 3.3f, 5f, 0.12f, 1000f, new MinMax(-300f, 300f), new MinMax(500f, 800f)), new Shark(1f, 1f, 2f, 240f, 310f, 1.5f, 50f), new DogFish(2f, 1f, 800f, 600f, 2f, 3, 4, new MinMax(1.3f, 2f), 400f), new Peashot(1f, 3, new string[2] { "D:1, P:2, D:1, P:1", "D:1, P:1, D:1, P:2" }, 1, 550f, 0.65f, "P,P,R"), new Barrel(1f, 3.6f, 0.9f, 1f, 3f, 0.8f), new Cannon(true, 1f, 850f, new MinMax(3.4f, 4.5f)), new Boat(0f, 200f, 1f, 2f, 0f, 0f, 0f, 0, 0f, 0f, 0f, 0f)));
				list.Add(new State(0.22f, new Pattern[1][] { new Pattern[1] { Pattern.Boat } }, States.Boat, new Squid(2f, 1, new MinMax(31f, 61f), 5.5f, new MinMax(-200f, -60f), 0.4f, 0.4f, 3.3f, 5f, 0.12f, 1000f, new MinMax(-300f, 300f), new MinMax(500f, 800f)), new Shark(1f, 1f, 2f, 240f, 310f, 1.5f, 50f), new DogFish(2f, 1f, 800f, 600f, 2f, 3, 4, new MinMax(1.3f, 2f), 400f), new Peashot(1f, 3, new string[2] { "D:1, P:2, D:1, P:1", "D:1, P:1, D:1, P:2" }, 1, 550f, 0.65f, "P,P,R"), new Barrel(1f, 3.6f, 0.9f, 1f, 3f, 0.8f), new Cannon(true, 1f, 850f, new MinMax(3.4f, 4.5f)), new Boat(3f, 0.7f, 2f, 2f, 300f, 360f, 0.7f, 3, 2f, 1f, 3f, 2f)));
				break;
			case Level.Mode.Hard:
				hp = 1400;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] { Pattern.Peashot } }, States.Main, new Squid(1f, 1, new MinMax(45f, 71f), 5.5f, new MinMax(-150f, -60f), 0.4f, 0.4f, 4f, 5f, 0.12f, 1000f, new MinMax(-260f, 330f), new MinMax(550f, 850f)), new Shark(1f, 1f, 1.8f, 290f, 340f, 1f, 150f), new DogFish(1f, 1f, 800f, 600f, 1.5f, 3, 4, new MinMax(0.9f, 1.3f), 400f), new Peashot(2f, 3, new string[5] { "D:0.5,P:2,D:1,P3", "D:0.5,P:2,D:0.5,P:3", "D:1,P:3,D:1,P:2", "D:0.5,P:3,D:1.5,P:2", "D:1,P:4" }, 1, 650f, 0.55f, "P,P,R,P,R"), new Barrel(1f, 2.7f, 0.9f, 1f, 2.3f, 0.8f), new Cannon(false, 1f, 850f, new MinMax(3f, 4f)), new Boat(3f, 0.7f, 2f, 2f, 355f, 460f, 0.6f, 2, 1.5f, 1f, 2.4f, 1f)));
				list.Add(new State(0.92f, new Pattern[2][]
				{
					new Pattern[6]
					{
						Pattern.Peashot,
						Pattern.Squid,
						Pattern.Peashot,
						Pattern.Shark,
						Pattern.Peashot,
						Pattern.DogFish
					},
					new Pattern[6]
					{
						Pattern.Shark,
						Pattern.Peashot,
						Pattern.Squid,
						Pattern.Peashot,
						Pattern.DogFish,
						Pattern.Peashot
					}
				}, States.Generic, new Squid(1f, 1, new MinMax(45f, 71f), 5.5f, new MinMax(-150f, -60f), 0.4f, 0.4f, 4f, 5f, 0.12f, 1000f, new MinMax(-260f, 330f), new MinMax(550f, 850f)), new Shark(1f, 1f, 1.8f, 290f, 340f, 1f, 150f), new DogFish(1f, 1f, 800f, 600f, 1.5f, 3, 4, new MinMax(0.9f, 1.3f), 400f), new Peashot(2f, 1, new string[5] { "P:3,D:0.5,P:1", "P:4", "P:2,D:1,P:2", "P:1,D:0.5,P:3", "P:4" }, 1, 660f, 0.5f, "P,R"), new Barrel(1f, 2.7f, 0.9f, 1f, 2.3f, 0.8f), new Cannon(false, 1f, 850f, new MinMax(3f, 4f)), new Boat(3f, 0.7f, 2f, 2f, 355f, 460f, 0.6f, 2, 1.5f, 1f, 2.4f, 1f)));
				list.Add(new State(0.77f, new Pattern[2][]
				{
					new Pattern[6]
					{
						Pattern.Peashot,
						Pattern.Squid,
						Pattern.Peashot,
						Pattern.Shark,
						Pattern.Peashot,
						Pattern.DogFish
					},
					new Pattern[6]
					{
						Pattern.Shark,
						Pattern.Peashot,
						Pattern.Squid,
						Pattern.Peashot,
						Pattern.DogFish,
						Pattern.Peashot
					}
				}, States.Generic, new Squid(1f, 1, new MinMax(45f, 71f), 5.5f, new MinMax(-150f, -60f), 0.4f, 0.4f, 4f, 5f, 0.12f, 1000f, new MinMax(-260f, 330f), new MinMax(550f, 850f)), new Shark(1f, 1f, 1.8f, 290f, 340f, 1f, 150f), new DogFish(1f, 1f, 800f, 600f, 1.5f, 3, 4, new MinMax(0.9f, 1.3f), 400f), new Peashot(2f, 1, new string[8] { "P:2", "P:2", "P:4", "P:3", "P:2", "P:4", "P:2", "P:3" }, 1, 670f, 0.45f, "P,R"), new Barrel(1f, 2.7f, 0.9f, 1f, 2.3f, 0.8f), new Cannon(true, 1f, 900f, new MinMax(2.5f, 3.9f)), new Boat(3f, 0.7f, 2f, 2f, 355f, 460f, 0.6f, 2, 1.5f, 1f, 2.4f, 1f)));
				list.Add(new State(0.32f, new Pattern[1][] { new Pattern[1] { Pattern.Boat } }, States.Boat, new Squid(1f, 1, new MinMax(45f, 71f), 5.5f, new MinMax(-150f, -60f), 0.4f, 0.4f, 4f, 5f, 0.12f, 1000f, new MinMax(-260f, 330f), new MinMax(550f, 850f)), new Shark(1f, 1f, 1.8f, 290f, 340f, 1f, 150f), new DogFish(1f, 1f, 800f, 600f, 1.5f, 3, 4, new MinMax(0.9f, 1.3f), 400f), new Peashot(2f, 1, new string[8] { "P:2", "P:2", "P:4", "P:3", "P:2", "P:4", "P:2", "P:3" }, 1, 670f, 0.45f, "P,R"), new Barrel(1f, 2.7f, 0.9f, 1f, 2.3f, 0.8f), new Cannon(true, 1f, 900f, new MinMax(2.5f, 3.9f)), new Boat(3f, 0.7f, 2f, 2f, 355f, 460f, 0.6f, 2, 1.5f, 1f, 2.4f, 1f)));
				break;
			}
			return new Pirate(hp, goalTimes, list.ToArray());
		}
	}

	public class RetroArcade : AbstractLevelProperties<RetroArcade.State, RetroArcade.Pattern, RetroArcade.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected RetroArcade properties { get; private set; }

			public virtual void LevelInit(RetroArcade properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			Caterpillar = 2,
			Robots = 3,
			PaddleShip = 4,
			QShip = 5,
			UFO = 6,
			Toad = 7,
			Worm = 8,
			Aliens = 9
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Aliens aliens;

			public readonly Caterpillar caterpillar;

			public readonly Robots robots;

			public readonly PaddleShip paddleShip;

			public readonly QShip qShip;

			public readonly UFO uFO;

			public readonly Toad toad;

			public readonly Worm worm;

			public readonly General general;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Aliens aliens, Caterpillar caterpillar, Robots robots, PaddleShip paddleShip, QShip qShip, UFO uFO, Toad toad, Worm worm, General general)
				: base(healthTrigger, patterns, stateName)
			{
				this.aliens = aliens;
				this.caterpillar = caterpillar;
				this.robots = robots;
				this.paddleShip = paddleShip;
				this.qShip = qShip;
				this.uFO = uFO;
				this.toad = toad;
				this.worm = worm;
				this.general = general;
			}
		}

		public class Aliens : AbstractLevelPropertyGroup
		{
			public readonly float hp;

			public readonly float moveTime;

			public readonly float moveTimeDecrease;

			public readonly int numColumns;

			public readonly MinMax shotRate;

			public readonly float shotRateDecrease;

			public readonly float randomShotAverageTime;

			public readonly string[] shotColumnPattern;

			public readonly float bulletSpeed;

			public readonly MinMax bonusAppearTime;

			public readonly int bonusAppearCount;

			public readonly float bonusMoveSpeed;

			public readonly float pointsGained;

			public readonly float pointsBonus;

			public Aliens(float hp, float moveTime, float moveTimeDecrease, int numColumns, MinMax shotRate, float shotRateDecrease, float randomShotAverageTime, string[] shotColumnPattern, float bulletSpeed, MinMax bonusAppearTime, int bonusAppearCount, float bonusMoveSpeed, float pointsGained, float pointsBonus)
			{
				this.hp = hp;
				this.moveTime = moveTime;
				this.moveTimeDecrease = moveTimeDecrease;
				this.numColumns = numColumns;
				this.shotRate = shotRate;
				this.shotRateDecrease = shotRateDecrease;
				this.randomShotAverageTime = randomShotAverageTime;
				this.shotColumnPattern = shotColumnPattern;
				this.bulletSpeed = bulletSpeed;
				this.bonusAppearTime = bonusAppearTime;
				this.bonusAppearCount = bonusAppearCount;
				this.bonusMoveSpeed = bonusMoveSpeed;
				this.pointsGained = pointsGained;
				this.pointsBonus = pointsBonus;
			}
		}

		public class Caterpillar : AbstractLevelPropertyGroup
		{
			public readonly float hp;

			public readonly float moveTime;

			public readonly float moveTimeDecrease;

			public readonly int[] bodyParts;

			public readonly float shotSpeed;

			public readonly int dropCount;

			public readonly MinMax spiderDelay;

			public readonly int spiderCount;

			public readonly float spiderSpeed;

			public readonly MinMax spiderPathY;

			public readonly int spiderNumZigZags;

			public readonly float pointsGained;

			public readonly float pointsBonus;

			public Caterpillar(float hp, float moveTime, float moveTimeDecrease, int[] bodyParts, float shotSpeed, int dropCount, MinMax spiderDelay, int spiderCount, float spiderSpeed, MinMax spiderPathY, int spiderNumZigZags, float pointsGained, float pointsBonus)
			{
				this.hp = hp;
				this.moveTime = moveTime;
				this.moveTimeDecrease = moveTimeDecrease;
				this.bodyParts = bodyParts;
				this.shotSpeed = shotSpeed;
				this.dropCount = dropCount;
				this.spiderDelay = spiderDelay;
				this.spiderCount = spiderCount;
				this.spiderSpeed = spiderSpeed;
				this.spiderPathY = spiderPathY;
				this.spiderNumZigZags = spiderNumZigZags;
				this.pointsGained = pointsGained;
				this.pointsBonus = pointsBonus;
			}
		}

		public class Robots : AbstractLevelPropertyGroup
		{
			public readonly float mainRobotHp;

			public readonly string mainRobotShootString;

			public readonly float mainRobotShootSpeed;

			public readonly bool mainRobotShotBounce;

			public readonly float mainRobotMoveSpeed;

			public readonly MinMax mainRobotY;

			public readonly float smallRobotHp;

			public readonly MinMax smallRobotAttackDelay;

			public readonly float smallRobotShootSpeed;

			public readonly float smallRobotRotationSpeed;

			public readonly float smallRobotRotationDistance;

			public readonly MinMax bonusDelay;

			public readonly int bonusCount;

			public readonly float bonusMoveSpeed;

			public readonly float bonusHp;

			public readonly float pointsGained;

			public readonly float pointsBonus;

			public readonly string[] robotWaves;

			public readonly float[] robotsXPositions;

			public readonly string[] robotColorPattern;

			public Robots(float mainRobotHp, string mainRobotShootString, float mainRobotShootSpeed, bool mainRobotShotBounce, float mainRobotMoveSpeed, MinMax mainRobotY, float smallRobotHp, MinMax smallRobotAttackDelay, float smallRobotShootSpeed, float smallRobotRotationSpeed, float smallRobotRotationDistance, MinMax bonusDelay, int bonusCount, float bonusMoveSpeed, float bonusHp, float pointsGained, float pointsBonus, string[] robotWaves, float[] robotsXPositions, string[] robotColorPattern)
			{
				this.mainRobotHp = mainRobotHp;
				this.mainRobotShootString = mainRobotShootString;
				this.mainRobotShootSpeed = mainRobotShootSpeed;
				this.mainRobotShotBounce = mainRobotShotBounce;
				this.mainRobotMoveSpeed = mainRobotMoveSpeed;
				this.mainRobotY = mainRobotY;
				this.smallRobotHp = smallRobotHp;
				this.smallRobotAttackDelay = smallRobotAttackDelay;
				this.smallRobotShootSpeed = smallRobotShootSpeed;
				this.smallRobotRotationSpeed = smallRobotRotationSpeed;
				this.smallRobotRotationDistance = smallRobotRotationDistance;
				this.bonusDelay = bonusDelay;
				this.bonusCount = bonusCount;
				this.bonusMoveSpeed = bonusMoveSpeed;
				this.bonusHp = bonusHp;
				this.pointsGained = pointsGained;
				this.pointsBonus = pointsBonus;
				this.robotWaves = robotWaves;
				this.robotsXPositions = robotsXPositions;
				this.robotColorPattern = robotColorPattern;
			}
		}

		public class PaddleShip : AbstractLevelPropertyGroup
		{
			public readonly float hp;

			public readonly MinMax ySpeed;

			public readonly float xSpeed;

			public readonly float damageMultiplier;

			public readonly float pointsGained;

			public readonly float pointsBonus;

			public PaddleShip(float hp, MinMax ySpeed, float xSpeed, float damageMultiplier, float pointsGained, float pointsBonus)
			{
				this.hp = hp;
				this.ySpeed = ySpeed;
				this.xSpeed = xSpeed;
				this.damageMultiplier = damageMultiplier;
				this.pointsGained = pointsGained;
				this.pointsBonus = pointsBonus;
			}
		}

		public class QShip : AbstractLevelPropertyGroup
		{
			public readonly float moveSpeed;

			public readonly int numSpinningTiles;

			public readonly MinMax tileRotationSpeed;

			public readonly float tileRotationDistance;

			public readonly float shotSpreadAngle;

			public readonly float shotSpeed;

			public readonly float maxXPos;

			public readonly float yPos;

			public readonly float hp;

			public readonly float damageMultiplier;

			public readonly float tentacleWarningDuration;

			public readonly MinMax tentacleSpawnRange;

			public readonly float tentacleSpeed;

			public readonly float pointsGained;

			public readonly float pointsBonus;

			public QShip(float moveSpeed, int numSpinningTiles, MinMax tileRotationSpeed, float tileRotationDistance, float shotSpreadAngle, float shotSpeed, float maxXPos, float yPos, float hp, float damageMultiplier, float tentacleWarningDuration, MinMax tentacleSpawnRange, float tentacleSpeed, float pointsGained, float pointsBonus)
			{
				this.moveSpeed = moveSpeed;
				this.numSpinningTiles = numSpinningTiles;
				this.tileRotationSpeed = tileRotationSpeed;
				this.tileRotationDistance = tileRotationDistance;
				this.shotSpreadAngle = shotSpreadAngle;
				this.shotSpeed = shotSpeed;
				this.maxXPos = maxXPos;
				this.yPos = yPos;
				this.hp = hp;
				this.damageMultiplier = damageMultiplier;
				this.tentacleWarningDuration = tentacleWarningDuration;
				this.tentacleSpawnRange = tentacleSpawnRange;
				this.tentacleSpeed = tentacleSpeed;
				this.pointsGained = pointsGained;
				this.pointsBonus = pointsBonus;
			}
		}

		public class UFO : AbstractLevelPropertyGroup
		{
			public readonly float projectileSpeed;

			public readonly MinMax shotRate;

			public readonly int turretCount;

			public readonly float turretSpeed;

			public readonly float hp;

			public readonly MinMax bossSpeed;

			public readonly float moleSpeed;

			public readonly float moleWarningDelay;

			public readonly float moleAttackSpeed;

			public readonly float pointsGained;

			public readonly float pointsBonus;

			public readonly float initialPositionX;

			public readonly float[] cyclePositionX;

			public readonly int alienYPosition;

			public UFO(float projectileSpeed, MinMax shotRate, int turretCount, float turretSpeed, float hp, MinMax bossSpeed, float moleSpeed, float moleWarningDelay, float moleAttackSpeed, float pointsGained, float pointsBonus, float initialPositionX, float[] cyclePositionX, int alienYPosition)
			{
				this.projectileSpeed = projectileSpeed;
				this.shotRate = shotRate;
				this.turretCount = turretCount;
				this.turretSpeed = turretSpeed;
				this.hp = hp;
				this.bossSpeed = bossSpeed;
				this.moleSpeed = moleSpeed;
				this.moleWarningDelay = moleWarningDelay;
				this.moleAttackSpeed = moleAttackSpeed;
				this.pointsGained = pointsGained;
				this.pointsBonus = pointsBonus;
				this.initialPositionX = initialPositionX;
				this.cyclePositionX = cyclePositionX;
				this.alienYPosition = alienYPosition;
			}
		}

		public class Toad : AbstractLevelPropertyGroup
		{
			public readonly float hp;

			public readonly MinMax initialAttackDelay;

			public readonly float attackDelay;

			public readonly float attackDelayDecrease;

			public readonly float shotSpeed;

			public readonly float carSpeed;

			public readonly string carTypes;

			public readonly float carDelay;

			public readonly float missileSpeed;

			public readonly float pointsGained;

			public readonly float pointsBonus;

			public Toad(float hp, MinMax initialAttackDelay, float attackDelay, float attackDelayDecrease, float shotSpeed, float carSpeed, string carTypes, float carDelay, float missileSpeed, float pointsGained, float pointsBonus)
			{
				this.hp = hp;
				this.initialAttackDelay = initialAttackDelay;
				this.attackDelay = attackDelay;
				this.attackDelayDecrease = attackDelayDecrease;
				this.shotSpeed = shotSpeed;
				this.carSpeed = carSpeed;
				this.carTypes = carTypes;
				this.carDelay = carDelay;
				this.missileSpeed = missileSpeed;
				this.pointsGained = pointsGained;
				this.pointsBonus = pointsBonus;
			}
		}

		public class Worm : AbstractLevelPropertyGroup
		{
			public readonly MinMax moveSpeed;

			public readonly float rocketSpeed;

			public readonly float rocketSpawnDelay;

			public readonly float rocketBrokenPieceSpeed;

			public readonly float hp;

			public readonly float tongueRotateSpeed;

			public readonly float pointsGained;

			public readonly float pointsBonus;

			public Worm(MinMax moveSpeed, float rocketSpeed, float rocketSpawnDelay, float rocketBrokenPieceSpeed, float hp, float tongueRotateSpeed, float pointsGained, float pointsBonus)
			{
				this.moveSpeed = moveSpeed;
				this.rocketSpeed = rocketSpeed;
				this.rocketSpawnDelay = rocketSpawnDelay;
				this.rocketBrokenPieceSpeed = rocketBrokenPieceSpeed;
				this.hp = hp;
				this.tongueRotateSpeed = tongueRotateSpeed;
				this.pointsGained = pointsGained;
				this.pointsBonus = pointsBonus;
			}
		}

		public class General : AbstractLevelPropertyGroup
		{
			public readonly float accuracyBonus;

			public General(float accuracyBonus)
			{
				this.accuracyBonus = accuracyBonus;
			}
		}

		public RetroArcade(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				timeline.events.Add(new Level.Timeline.Event("Caterpillar", 0.92f));
				timeline.events.Add(new Level.Timeline.Event("Robots", 0.83f));
				timeline.events.Add(new Level.Timeline.Event("PaddleShip", 0.75f));
				timeline.events.Add(new Level.Timeline.Event("Aliens", 0.63f));
				timeline.events.Add(new Level.Timeline.Event("Caterpillar", 0.58f));
				timeline.events.Add(new Level.Timeline.Event("Robots", 0.5f));
				timeline.events.Add(new Level.Timeline.Event("UFO", 0.41f));
				timeline.events.Add(new Level.Timeline.Event("Aliens", 0.33f));
				timeline.events.Add(new Level.Timeline.Event("Caterpillar", 0.25f));
				timeline.events.Add(new Level.Timeline.Event("Robots", 0.16f));
				timeline.events.Add(new Level.Timeline.Event("QShip", 0.08f));
				break;
			case Level.Mode.Normal:
				timeline.health = 100f;
				timeline.events.Add(new Level.Timeline.Event("Caterpillar", 0.92f));
				timeline.events.Add(new Level.Timeline.Event("Robots", 0.84f));
				timeline.events.Add(new Level.Timeline.Event("PaddleShip", 0.76f));
				timeline.events.Add(new Level.Timeline.Event("Aliens", 0.68f));
				timeline.events.Add(new Level.Timeline.Event("Caterpillar", 0.6f));
				timeline.events.Add(new Level.Timeline.Event("Robots", 0.52f));
				timeline.events.Add(new Level.Timeline.Event("UFO", 0.44f));
				timeline.events.Add(new Level.Timeline.Event("Aliens", 0.35f));
				timeline.events.Add(new Level.Timeline.Event("Caterpillar", 0.26f));
				timeline.events.Add(new Level.Timeline.Event("Robots", 0.17f));
				timeline.events.Add(new Level.Timeline.Event("QShip", 0.08f));
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				timeline.events.Add(new Level.Timeline.Event("Caterpillar", 0.92f));
				timeline.events.Add(new Level.Timeline.Event("Robots", 0.83f));
				timeline.events.Add(new Level.Timeline.Event("PaddleShip", 0.75f));
				timeline.events.Add(new Level.Timeline.Event("Aliens", 0.63f));
				timeline.events.Add(new Level.Timeline.Event("Caterpillar", 0.58f));
				timeline.events.Add(new Level.Timeline.Event("Robots", 0.5f));
				timeline.events.Add(new Level.Timeline.Event("UFO", 0.41f));
				timeline.events.Add(new Level.Timeline.Event("Aliens", 0.33f));
				timeline.events.Add(new Level.Timeline.Event("Caterpillar", 0.25f));
				timeline.events.Add(new Level.Timeline.Event("Robots", 0.16f));
				timeline.events.Add(new Level.Timeline.Event("QShip", 0.08f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern RetroArcade.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static RetroArcade GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.92f, new Pattern[1][] { new Pattern[0] }, States.Caterpillar, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.83f, new Pattern[1][] { new Pattern[0] }, States.Robots, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.75f, new Pattern[1][] { new Pattern[0] }, States.PaddleShip, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.63f, new Pattern[1][] { new Pattern[0] }, States.Aliens, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.58f, new Pattern[1][] { new Pattern[0] }, States.Caterpillar, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.5f, new Pattern[1][] { new Pattern[0] }, States.Robots, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.41f, new Pattern[1][] { new Pattern[0] }, States.UFO, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.33f, new Pattern[1][] { new Pattern[0] }, States.Aliens, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.25f, new Pattern[1][] { new Pattern[0] }, States.Caterpillar, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.16f, new Pattern[1][] { new Pattern[0] }, States.Robots, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.08f, new Pattern[1][] { new Pattern[0] }, States.QShip, new Aliens(1f, 5.6f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 8f, new string[3] { "1,2,5,6,4,3,6", "5,3,4,1,6,2,4", "3,6,1,2,4,5,2" }, 225f, new MinMax(6f, 8f), 1, 200f, 1f, 5f), new Caterpillar(1f, 1.5f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 1, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 6, 1f, 5f), new Robots(15f, "3.6,2.5,3,2.5,2,3.8,3.2,3,3.3,2.2,3,2.4,3.2", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 9f), 1, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(40f, new MinMax(100f, 150f), 2.75f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(80f, 220f), 120f, 60f, 250f, 200f, 140f, 180f, 10f, 1f, new MinMax(1f, 2f), 350f, 1f, 5f), new UFO(225f, new MinMax(3f, 2f), 8, 137f, 20f, new MinMax(200f, 400f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				break;
			case Level.Mode.Normal:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Aliens(1f, 3.7f, 0.25f, 3, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[5] { "2,1,3", "3,2,1", "2,3,1", "3,1,2", "1,2,3" }, 300f, new MinMax(2.5f, 3.5f), 1, 250f, 25f, 100f), new Caterpillar(1f, 1.55f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 2, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "1,1.8,1.6,1.3,1.9,1.4,1.4,1.9,1.6,1.7", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 150f, 80f, new MinMax(3f, 4f), 1, 200f, 1f, 25f, 100f, new string[1] { "2" }, new float[3] { -213f, 0f, 213f }, new string[1] { "1-1-1" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.92f, new Pattern[1][] { new Pattern[0] }, States.Caterpillar, new Aliens(1f, 3.7f, 0.25f, 3, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[5] { "2,1,3", "3,2,1", "2,3,1", "3,1,2", "1,2,3" }, 300f, new MinMax(2.5f, 3.5f), 1, 250f, 25f, 100f), new Caterpillar(1f, 1.55f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 2, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "1,1.8,1.6,1.3,1.9,1.4,1.4,1.9,1.6,1.7", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 150f, 80f, new MinMax(3f, 4f), 1, 200f, 1f, 25f, 100f, new string[1] { "2" }, new float[3] { -213f, 0f, 213f }, new string[1] { "1-1-1" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.84f, new Pattern[1][] { new Pattern[0] }, States.Robots, new Aliens(1f, 3.7f, 0.25f, 3, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[5] { "2,1,3", "3,2,1", "2,3,1", "3,1,2", "1,2,3" }, 300f, new MinMax(2.5f, 3.5f), 1, 250f, 25f, 100f), new Caterpillar(1f, 1.55f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 2, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "1,1.8,1.6,1.3,1.9,1.4,1.4,1.9,1.6,1.7", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 150f, 80f, new MinMax(3f, 4f), 1, 200f, 1f, 25f, 100f, new string[1] { "2" }, new float[3] { -213f, 0f, 213f }, new string[1] { "1-1-1" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.76f, new Pattern[1][] { new Pattern[0] }, States.PaddleShip, new Aliens(1f, 3.7f, 0.25f, 3, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[5] { "2,1,3", "3,2,1", "2,3,1", "3,1,2", "1,2,3" }, 300f, new MinMax(2.5f, 3.5f), 1, 250f, 25f, 100f), new Caterpillar(1f, 1.55f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 2, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "1,1.8,1.6,1.3,1.9,1.4,1.4,1.9,1.6,1.7", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 150f, 80f, new MinMax(3f, 4f), 1, 200f, 1f, 25f, 100f, new string[1] { "2" }, new float[3] { -213f, 0f, 213f }, new string[1] { "1-1-1" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.68f, new Pattern[1][] { new Pattern[0] }, States.Aliens, new Aliens(1f, 4.5f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[4] { "1,5,2,6,4,3", "5,1,4,2,6,3", "2,6,4,3,5,1", "4,6,1,3,5,2" }, 300f, new MinMax(4f, 6f), 1, 300f, 25f, 100f), new Caterpillar(1f, 1.55f, 0.05f, new int[6] { 1, 1, 1, 2, 2, 2 }, 400f, 2, new MinMax(0.3f, 0.8f), 1, 400f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "1,1.8,1.6,1.3,1.9,1.4,1.4,1.9,1.6,1.7", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 150f, 80f, new MinMax(3f, 4f), 1, 200f, 1f, 25f, 100f, new string[1] { "2" }, new float[3] { -213f, 0f, 213f }, new string[1] { "1-1-1" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.6f, new Pattern[1][] { new Pattern[0] }, States.Caterpillar, new Aliens(1f, 4.5f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[4] { "1,5,2,6,4,3", "5,1,4,2,6,3", "2,6,4,3,5,1", "4,6,1,3,5,2" }, 300f, new MinMax(4f, 6f), 1, 300f, 25f, 100f), new Caterpillar(1f, 1.7f, 0.08f, new int[9] { 1, 1, 1, 2, 2, 2, 3, 3, 3 }, 400f, 2, new MinMax(0.3f, 0.8f), 1, 450f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "1,1.8,1.6,1.3,1.9,1.4,1.4,1.9,1.6,1.7", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 150f, 80f, new MinMax(3f, 4f), 1, 200f, 1f, 25f, 100f, new string[1] { "2" }, new float[3] { -213f, 0f, 213f }, new string[1] { "1-1-1" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.52f, new Pattern[1][] { new Pattern[0] }, States.Robots, new Aliens(1f, 4.5f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[4] { "1,5,2,6,4,3", "5,1,4,2,6,3", "2,6,4,3,5,1", "4,6,1,3,5,2" }, 300f, new MinMax(4f, 6f), 1, 300f, 25f, 100f), new Caterpillar(1f, 1.7f, 0.08f, new int[9] { 1, 1, 1, 2, 2, 2, 3, 3, 3 }, 400f, 2, new MinMax(0.3f, 0.8f), 1, 450f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "2,3.1,2.5,2.8,2.5,2,3,2.7,2,3,3.3,2.2,2.65,2.4,3.2,1.5,2.8,2,3", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 160f, 80f, new MinMax(5f, 7f), 1, 225f, 1f, 25f, 100f, new string[1] { "1,3" }, new float[3] { -213f, 0f, 213f }, new string[1] { "1-2-2,2-1-1" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.44f, new Pattern[1][] { new Pattern[0] }, States.UFO, new Aliens(1f, 4.5f, 0.2f, 6, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[4] { "1,5,2,6,4,3", "5,1,4,2,6,3", "2,6,4,3,5,1", "4,6,1,3,5,2" }, 300f, new MinMax(4f, 6f), 1, 300f, 25f, 100f), new Caterpillar(1f, 1.7f, 0.08f, new int[9] { 1, 1, 1, 2, 2, 2, 3, 3, 3 }, 400f, 2, new MinMax(0.3f, 0.8f), 1, 450f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "2,3.1,2.5,2.8,2.5,2,3,2.7,2,3,3.3,2.2,2.65,2.4,3.2,1.5,2.8,2,3", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 160f, 80f, new MinMax(5f, 7f), 1, 225f, 1f, 25f, 100f, new string[1] { "1,3" }, new float[3] { -213f, 0f, 213f }, new string[1] { "1-2-2,2-1-1" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.35f, new Pattern[1][] { new Pattern[0] }, States.Aliens, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[4] { "3,5,2,6,1,7,4,8,9", "7,5,2,4,9,3,8,6,1", "8,5,9,1,6,2,7,3,4", "2,8,6,1,3,9,7,5,4" }, 300f, new MinMax(6f, 8f), 2, 350f, 25f, 100f), new Caterpillar(1f, 1.7f, 0.08f, new int[9] { 1, 1, 1, 2, 2, 2, 3, 3, 3 }, 400f, 2, new MinMax(0.3f, 0.8f), 1, 450f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "2,3.1,2.5,2.8,2.5,2,3,2.7,2,3,3.3,2.2,2.65,2.4,3.2,1.5,2.8,2,3", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 160f, 80f, new MinMax(5f, 7f), 1, 225f, 1f, 25f, 100f, new string[1] { "1,3" }, new float[3] { -213f, 0f, 213f }, new string[1] { "1-2-2,2-1-1" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.26f, new Pattern[1][] { new Pattern[0] }, States.Caterpillar, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[4] { "3,5,2,6,1,7,4,8,9", "7,5,2,4,9,3,8,6,1", "8,5,9,1,6,2,7,3,4", "2,8,6,1,3,9,7,5,4" }, 300f, new MinMax(6f, 8f), 2, 350f, 25f, 100f), new Caterpillar(1f, 1.65f, 0.05f, new int[12]
				{
					1, 1, 1, 2, 2, 2, 3, 3, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.5f, 0.8f), 2, 500f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "2,3.1,2.5,2.8,2.5,2,3,2.7,2,3,3.3,2.2,2.65,2.4,3.2,1.5,2.8,2,3", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 160f, 80f, new MinMax(5f, 7f), 1, 225f, 1f, 25f, 100f, new string[1] { "1,3" }, new float[3] { -213f, 0f, 213f }, new string[1] { "1-2-2,2-1-1" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.17f, new Pattern[1][] { new Pattern[0] }, States.Robots, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[4] { "3,5,2,6,1,7,4,8,9", "7,5,2,4,9,3,8,6,1", "8,5,9,1,6,2,7,3,4", "2,8,6,1,3,9,7,5,4" }, 300f, new MinMax(6f, 8f), 2, 350f, 25f, 100f), new Caterpillar(1f, 1.65f, 0.05f, new int[12]
				{
					1, 1, 1, 2, 2, 2, 3, 3, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.5f, 0.8f), 2, 500f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "2,3.6,2.5,3,2.5,2,3,3.2,2,4,3.3,2.2,3,2.4,3.2,1.5,2.8,2,3", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 170f, 80f, new MinMax(6f, 7f), 2, 250f, 1f, 25f, 100f, new string[1] { "1,2,3" }, new float[3] { 213f, 0f, -213f }, new string[1] { "1-2-3,1-2-3,1-2-3" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.08f, new Pattern[1][] { new Pattern[0] }, States.QShip, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(2f, 2.5f), 0.05f, 9999f, new string[4] { "3,5,2,6,1,7,4,8,9", "7,5,2,4,9,3,8,6,1", "8,5,9,1,6,2,7,3,4", "2,8,6,1,3,9,7,5,4" }, 300f, new MinMax(6f, 8f), 2, 350f, 25f, 100f), new Caterpillar(1f, 1.65f, 0.05f, new int[12]
				{
					1, 1, 1, 2, 2, 2, 3, 3, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.5f, 0.8f), 2, 500f, new MinMax(125f, 250f), 7, 25f, 100f), new Robots(15f, "2,3.6,2.5,3,2.5,2,3,3.2,2,4,3.3,2.2,3,2.4,3.2,1.5,2.8,2,3", 260f, false, 125f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 9999f, 170f, 80f, new MinMax(6f, 7f), 2, 250f, 1f, 25f, 100f, new string[1] { "1,2,3" }, new float[3] { 213f, 0f, -213f }, new string[1] { "1-2-3,1-2-3,1-2-3" }), new PaddleShip(16f, new MinMax(450f, 500f), 315f, 1f, 100f, 0f), new QShip(100f, 3, new MinMax(100f, 250f), 100f, 60f, 400f, 200f, 160f, 60f, 1f, 1f, new MinMax(1f, 2f), 335f, 55f, 0f), new UFO(225f, new MinMax(1f, 0.8f), 8, 37f, 12f, new MinMax(450f, 450f), 0f, 9999f, 0f, 125f, 0f, 0f, new float[2] { 225f, -225f }, -3), new Toad(12f, new MinMax(1f, 3f), 2.2f, 0.2f, 150f, 200f, "M,R,M", 1.5f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.92f, new Pattern[1][] { new Pattern[0] }, States.Caterpillar, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.83f, new Pattern[1][] { new Pattern[0] }, States.Robots, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.75f, new Pattern[1][] { new Pattern[0] }, States.PaddleShip, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.63f, new Pattern[1][] { new Pattern[0] }, States.Aliens, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.58f, new Pattern[1][] { new Pattern[0] }, States.Caterpillar, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.5f, new Pattern[1][] { new Pattern[0] }, States.Robots, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.41f, new Pattern[1][] { new Pattern[0] }, States.UFO, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.33f, new Pattern[1][] { new Pattern[0] }, States.Aliens, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.25f, new Pattern[1][] { new Pattern[0] }, States.Caterpillar, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.16f, new Pattern[1][] { new Pattern[0] }, States.Robots, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				list.Add(new State(0.08f, new Pattern[1][] { new Pattern[0] }, States.QShip, new Aliens(1f, 5.9f, 0.2f, 9, new MinMax(1.8f, 2.3f), 0.05f, 7f, new string[3] { "3,5,2,6,9,1,7,4,8", "2,8,5,1,6,9,3,5,4", "5,6,1,9,7,8,3,2,4" }, 225f, new MinMax(6f, 8f), 2, 200f, 1f, 5f), new Caterpillar(1f, 1.9f, 0.05f, new int[12]
				{
					1, 2, 3, 1, 2, 3, 1, 2, 3, 1,
					1, 1
				}, 400f, 2, new MinMax(0.3f, 0.8f), 2, 400f, new MinMax(125f, 250f), 7, 1f, 5f), new Robots(15f, "2,2.5,3,2.5,2,3.2,2,3,3.3,2.2,3,2.4,3.5,1.5,2.8,2,3", 250f, false, 100f, new MinMax(100f, 200f), 1f, new MinMax(9998f, 9999f), 150f, 150f, 80f, new MinMax(6f, 8f), 2, 150f, 1f, 1f, 5f, new string[3] { "2", "1,3", "1,2,3" }, new float[3] { -213f, 0f, 213f }, new string[3] { "1-1-1", "1-2-2,2-2-1", "1-2-3,1-2-3,1-2-3" }), new PaddleShip(57f, new MinMax(100f, 175f), 350f, 3.6f, 1f, 5f), new QShip(100f, 3, new MinMax(100f, 300f), 120f, 60f, 250f, 200f, 140f, 230f, 10f, 1f, new MinMax(1f, 2f), 400f, 1f, 5f), new UFO(225f, new MinMax(2f, 1f), 8, 137f, 24f, new MinMax(300f, 500f), 200f, 0.5f, 150f, 1f, 5f, 0f, new float[2] { 250f, -250f }, -7), new Toad(12f, new MinMax(1f, 3f), 2f, 0.2f, 150f, 240f, "M,R,M", 1.25f, 500f, 1f, 5f), new Worm(new MinMax(100f, 400f), 300f, 0.75f, 500f, 6f, 200f, 1f, 5f), new General(2f)));
				break;
			}
			return new RetroArcade(hp, goalTimes, list.ToArray());
		}
	}

	public class Robot : AbstractLevelProperties<Robot.State, Robot.Pattern, Robot.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Robot properties { get; private set; }

			public virtual void LevelInit(Robot properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			HeliHead = 2,
			Inventor = 3
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Hose hose;

			public readonly ShotBot shotBot;

			public readonly Cannon cannon;

			public readonly Orb orb;

			public readonly Arms arms;

			public readonly MagnetArms magnetArms;

			public readonly TwistyArms twistyArms;

			public readonly BombBot bombBot;

			public readonly Heart heart;

			public readonly HeliHead heliHead;

			public readonly BlueGem blueGem;

			public readonly RedGem redGem;

			public readonly Inventor inventor;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Hose hose, ShotBot shotBot, Cannon cannon, Orb orb, Arms arms, MagnetArms magnetArms, TwistyArms twistyArms, BombBot bombBot, Heart heart, HeliHead heliHead, BlueGem blueGem, RedGem redGem, Inventor inventor)
				: base(healthTrigger, patterns, stateName)
			{
				this.hose = hose;
				this.shotBot = shotBot;
				this.cannon = cannon;
				this.orb = orb;
				this.arms = arms;
				this.magnetArms = magnetArms;
				this.twistyArms = twistyArms;
				this.bombBot = bombBot;
				this.heart = heart;
				this.heliHead = heliHead;
				this.blueGem = blueGem;
				this.redGem = redGem;
				this.inventor = inventor;
			}
		}

		public class Hose : AbstractLevelPropertyGroup
		{
			public readonly int health;

			public readonly float warningDuration;

			public readonly int beamDuration;

			public readonly MinMax attackDelayRange;

			public readonly MinMax aimAngleParameter;

			public readonly float delayMinus;

			public Hose(int health, float warningDuration, int beamDuration, MinMax attackDelayRange, MinMax aimAngleParameter, float delayMinus)
			{
				this.health = health;
				this.warningDuration = warningDuration;
				this.beamDuration = beamDuration;
				this.attackDelayRange = attackDelayRange;
				this.aimAngleParameter = aimAngleParameter;
				this.delayMinus = delayMinus;
			}
		}

		public class ShotBot : AbstractLevelPropertyGroup
		{
			public readonly int hatchGateHealth;

			public readonly int shotbotHealth;

			public readonly int bulletSpeed;

			public readonly int shotbotFlightSpeed;

			public readonly MinMax pinkBulletCount;

			public readonly int shotbotCount;

			public readonly float shotbotDelay;

			public readonly MinMax initialSpawnDelay;

			public readonly MinMax shotbotWaveDelay;

			public readonly float shotbotShootDelay;

			public readonly float shotbotSpawnDelayMinus;

			public ShotBot(int hatchGateHealth, int shotbotHealth, int bulletSpeed, int shotbotFlightSpeed, MinMax pinkBulletCount, int shotbotCount, float shotbotDelay, MinMax initialSpawnDelay, MinMax shotbotWaveDelay, float shotbotShootDelay, float shotbotSpawnDelayMinus)
			{
				this.hatchGateHealth = hatchGateHealth;
				this.shotbotHealth = shotbotHealth;
				this.bulletSpeed = bulletSpeed;
				this.shotbotFlightSpeed = shotbotFlightSpeed;
				this.pinkBulletCount = pinkBulletCount;
				this.shotbotCount = shotbotCount;
				this.shotbotDelay = shotbotDelay;
				this.initialSpawnDelay = initialSpawnDelay;
				this.shotbotWaveDelay = shotbotWaveDelay;
				this.shotbotShootDelay = shotbotShootDelay;
				this.shotbotSpawnDelayMinus = shotbotSpawnDelayMinus;
			}
		}

		public class Cannon : AbstractLevelPropertyGroup
		{
			public readonly float attackDelay;

			public readonly string[] spreadVariableGroups;

			public readonly string[] shootString;

			public readonly MinMax attackDelayRange;

			public Cannon(float attackDelay, string[] spreadVariableGroups, string[] shootString, MinMax attackDelayRange)
			{
				this.attackDelay = attackDelay;
				this.spreadVariableGroups = spreadVariableGroups;
				this.shootString = shootString;
				this.attackDelayRange = attackDelayRange;
			}
		}

		public class Orb : AbstractLevelPropertyGroup
		{
			public readonly int chestHP;

			public readonly int orbHP;

			public readonly int orbMovementSpeed;

			public readonly float orbInitialLaserDelay;

			public readonly MinMax orbInitialSpawnDelay;

			public readonly float orbSpawnDelay;

			public readonly float orbSpawnDelayMinus;

			public readonly bool orbShieldIsActive;

			public readonly float orbInitalOpenDelay;

			public Orb(int chestHP, int orbHP, int orbMovementSpeed, float orbInitialLaserDelay, MinMax orbInitialSpawnDelay, float orbSpawnDelay, float orbSpawnDelayMinus, bool orbShieldIsActive, float orbInitalOpenDelay)
			{
				this.chestHP = chestHP;
				this.orbHP = orbHP;
				this.orbMovementSpeed = orbMovementSpeed;
				this.orbInitialLaserDelay = orbInitialLaserDelay;
				this.orbInitialSpawnDelay = orbInitialSpawnDelay;
				this.orbSpawnDelay = orbSpawnDelay;
				this.orbSpawnDelayMinus = orbSpawnDelayMinus;
				this.orbShieldIsActive = orbShieldIsActive;
				this.orbInitalOpenDelay = orbInitalOpenDelay;
			}
		}

		public class Arms : AbstractLevelPropertyGroup
		{
			public readonly MinMax attackDelayRange;

			public readonly string attackString;

			public Arms(MinMax attackDelayRange, string attackString)
			{
				this.attackDelayRange = attackDelayRange;
				this.attackString = attackString;
			}
		}

		public class MagnetArms : AbstractLevelPropertyGroup
		{
			public readonly float magnetStartDelay;

			public readonly float magnetStayDelay;

			public readonly float magnetForce;

			public MagnetArms(float magnetStartDelay, float magnetStayDelay, float magnetForce)
			{
				this.magnetStartDelay = magnetStartDelay;
				this.magnetStayDelay = magnetStayDelay;
				this.magnetForce = magnetForce;
			}
		}

		public class TwistyArms : AbstractLevelPropertyGroup
		{
			public readonly float warningArmsMoveAmount;

			public readonly float warningDuration;

			public readonly float twistyMoveSpeed;

			public readonly float twistyArmsStayDuration;

			public readonly string twistyPositionString;

			public readonly float bulletSpeed;

			public readonly bool shootTwicePerCycle;

			public TwistyArms(float warningArmsMoveAmount, float warningDuration, float twistyMoveSpeed, float twistyArmsStayDuration, string twistyPositionString, float bulletSpeed, bool shootTwicePerCycle)
			{
				this.warningArmsMoveAmount = warningArmsMoveAmount;
				this.warningDuration = warningDuration;
				this.twistyMoveSpeed = twistyMoveSpeed;
				this.twistyArmsStayDuration = twistyArmsStayDuration;
				this.twistyPositionString = twistyPositionString;
				this.bulletSpeed = bulletSpeed;
				this.shootTwicePerCycle = shootTwicePerCycle;
			}
		}

		public class BombBot : AbstractLevelPropertyGroup
		{
			public readonly int bombHP;

			public readonly int initialBombMovementSpeed;

			public readonly float bombDelay;

			public readonly MinMax bombInitialMovementDuration;

			public readonly int bombBossDamage;

			public readonly int bombHomingSpeed;

			public readonly float bombRotationSpeed;

			public readonly int bombLifeTime;

			public BombBot(int bombHP, int initialBombMovementSpeed, float bombDelay, MinMax bombInitialMovementDuration, int bombBossDamage, int bombHomingSpeed, float bombRotationSpeed, int bombLifeTime)
			{
				this.bombHP = bombHP;
				this.initialBombMovementSpeed = initialBombMovementSpeed;
				this.bombDelay = bombDelay;
				this.bombInitialMovementDuration = bombInitialMovementDuration;
				this.bombBossDamage = bombBossDamage;
				this.bombHomingSpeed = bombHomingSpeed;
				this.bombRotationSpeed = bombRotationSpeed;
				this.bombLifeTime = bombLifeTime;
			}
		}

		public class Heart : AbstractLevelPropertyGroup
		{
			public readonly int heartHP;

			public readonly int heartDamageChangePercentage;

			public Heart(int heartHP, int heartDamageChangePercentage)
			{
				this.heartHP = heartHP;
				this.heartDamageChangePercentage = heartDamageChangePercentage;
			}
		}

		public class HeliHead : AbstractLevelPropertyGroup
		{
			public readonly int heliheadMovementSpeed;

			public readonly float offScreenDelay;

			public readonly float attackDelay;

			public readonly string onScreenHeight;

			public HeliHead(int heliheadMovementSpeed, float offScreenDelay, float attackDelay, string onScreenHeight)
			{
				this.heliheadMovementSpeed = heliheadMovementSpeed;
				this.offScreenDelay = offScreenDelay;
				this.attackDelay = attackDelay;
				this.onScreenHeight = onScreenHeight;
			}
		}

		public class BlueGem : AbstractLevelPropertyGroup
		{
			public readonly float robotRotationSpeed;

			public readonly int robotVerticalMovementSpeed;

			public readonly MinMax bulletSpeed;

			public readonly int bulletSpeedAcceleration;

			public readonly float bulletSpawnDelay;

			public readonly int bulletSineWaveStrength;

			public readonly float bulletWaveSpeedMultiplier;

			public readonly float bulletLifeTime;

			public readonly int numberOfSpawnPoints;

			public readonly bool gemWaveRotation;

			public readonly MinMax gemRotationRange;

			public BlueGem(float robotRotationSpeed, int robotVerticalMovementSpeed, MinMax bulletSpeed, int bulletSpeedAcceleration, float bulletSpawnDelay, int bulletSineWaveStrength, float bulletWaveSpeedMultiplier, float bulletLifeTime, int numberOfSpawnPoints, bool gemWaveRotation, MinMax gemRotationRange)
			{
				this.robotRotationSpeed = robotRotationSpeed;
				this.robotVerticalMovementSpeed = robotVerticalMovementSpeed;
				this.bulletSpeed = bulletSpeed;
				this.bulletSpeedAcceleration = bulletSpeedAcceleration;
				this.bulletSpawnDelay = bulletSpawnDelay;
				this.bulletSineWaveStrength = bulletSineWaveStrength;
				this.bulletWaveSpeedMultiplier = bulletWaveSpeedMultiplier;
				this.bulletLifeTime = bulletLifeTime;
				this.numberOfSpawnPoints = numberOfSpawnPoints;
				this.gemWaveRotation = gemWaveRotation;
				this.gemRotationRange = gemRotationRange;
			}
		}

		public class RedGem : AbstractLevelPropertyGroup
		{
			public readonly float robotRotationSpeed;

			public readonly int robotVerticalMovementSpeed;

			public readonly MinMax bulletSpeed;

			public readonly int bulletSpeedAcceleration;

			public readonly float bulletSpawnDelay;

			public readonly int bulletSineWaveStrength;

			public readonly float bulletWaveSpeedMultiplier;

			public readonly float bulletLifeTime;

			public readonly int numberOfSpawnPoints;

			public readonly bool gemWaveRotation;

			public readonly MinMax gemRotationRange;

			public RedGem(float robotRotationSpeed, int robotVerticalMovementSpeed, MinMax bulletSpeed, int bulletSpeedAcceleration, float bulletSpawnDelay, int bulletSineWaveStrength, float bulletWaveSpeedMultiplier, float bulletLifeTime, int numberOfSpawnPoints, bool gemWaveRotation, MinMax gemRotationRange)
			{
				this.robotRotationSpeed = robotRotationSpeed;
				this.robotVerticalMovementSpeed = robotVerticalMovementSpeed;
				this.bulletSpeed = bulletSpeed;
				this.bulletSpeedAcceleration = bulletSpeedAcceleration;
				this.bulletSpawnDelay = bulletSpawnDelay;
				this.bulletSineWaveStrength = bulletSineWaveStrength;
				this.bulletWaveSpeedMultiplier = bulletWaveSpeedMultiplier;
				this.bulletLifeTime = bulletLifeTime;
				this.numberOfSpawnPoints = numberOfSpawnPoints;
				this.gemWaveRotation = gemWaveRotation;
				this.gemRotationRange = gemRotationRange;
			}
		}

		public class Inventor : AbstractLevelPropertyGroup
		{
			public readonly float inventorIdleSpeedMultiplier;

			public readonly float initialAttackDelay;

			public readonly MinMax attackDuration;

			public readonly MinMax attackDelay;

			public readonly string gemColourString;

			public readonly int blockadeHorizontalSpawnOffset;

			public readonly int blockadeHorizontalSpeed;

			public readonly int blockadeVerticalSpeed;

			public readonly float blockadeGroupDelay;

			public readonly float blockadeIndividualDelay;

			public readonly int blockadeSegmentLength;

			public readonly int blockadeGroupSize;

			public Inventor(float inventorIdleSpeedMultiplier, float initialAttackDelay, MinMax attackDuration, MinMax attackDelay, string gemColourString, int blockadeHorizontalSpawnOffset, int blockadeHorizontalSpeed, int blockadeVerticalSpeed, float blockadeGroupDelay, float blockadeIndividualDelay, int blockadeSegmentLength, int blockadeGroupSize)
			{
				this.inventorIdleSpeedMultiplier = inventorIdleSpeedMultiplier;
				this.initialAttackDelay = initialAttackDelay;
				this.attackDuration = attackDuration;
				this.attackDelay = attackDelay;
				this.gemColourString = gemColourString;
				this.blockadeHorizontalSpawnOffset = blockadeHorizontalSpawnOffset;
				this.blockadeHorizontalSpeed = blockadeHorizontalSpeed;
				this.blockadeVerticalSpeed = blockadeVerticalSpeed;
				this.blockadeGroupDelay = blockadeGroupDelay;
				this.blockadeIndividualDelay = blockadeIndividualDelay;
				this.blockadeSegmentLength = blockadeSegmentLength;
				this.blockadeGroupSize = blockadeGroupSize;
			}
		}

		public Robot(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 500f;
				break;
			case Level.Mode.Normal:
				timeline.health = 1400f;
				timeline.events.Add(new Level.Timeline.Event("HeliHead", 0.9f));
				timeline.events.Add(new Level.Timeline.Event("Inventor", 0.8f));
				break;
			case Level.Mode.Hard:
				timeline.health = 1800f;
				timeline.events.Add(new Level.Timeline.Event("HeliHead", 0.9f));
				timeline.events.Add(new Level.Timeline.Event("Inventor", 0.8f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern Robot.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static Robot GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 500;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Hose(250, 2f, 2, new MinMax(4f, 7f), new MinMax(90f, 170f), 1f), new ShotBot(250, 7, 200, 380, new MinMax(0f, 2f), 2, 1.5f, new MinMax(1f, 2f), new MinMax(2.5f, 5f), 100f, 0.3f), new Cannon(2.5f, new string[4] { "S300,N4,120-240", "S300,N5,134-226", "S300,N4,125-245", "S300,N5,139-259" }, new string[2] { "S1,S2,S1,S2", "S4,S3,S4,S3" }, new MinMax(3f, 5.5f)), new Orb(250, 9999, 100, 1.2f, new MinMax(3f, 6f), 8f, 0.5f, true, 1f), new Arms(new MinMax(3f, 5f), "M"), new MagnetArms(2f, 4f, -850f), new TwistyArms(210f, 1.3f, 550f, 1f, "100,300,500,200,400", 200f, false), new BombBot(20, 200, 8f, new MinMax(1f, 3f), 25, 275, 2f, 2000), new Heart(500, 35), new HeliHead(700, 0.05f, 0.1f, "250,300,350,400,450,400,350,300"), new BlueGem(5f, 100, new MinMax(100f, 300f), 30, 1f, 100, 2f, 5f, 3, false, new MinMax(10f, 350f)), new RedGem(5f, 100, new MinMax(100f, 300f), 30, 1f, 100, 2f, 5f, 3, false, new MinMax(10f, 350f)), new Inventor(2f, 5f, new MinMax(10f, 15f), new MinMax(1f, 1.1f), "R,B,R,R,B", 0, 200, 100, 5f, 1.5f, 8, 2)));
				break;
			case Level.Mode.Normal:
				hp = 1400;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Hose(150, 1.5f, 2, new MinMax(0.01f, 0.011f), new MinMax(90f, 170f), 1f), new ShotBot(150, 7, 200, 380, new MinMax(0f, 2f), 4, 1f, new MinMax(1f, 2.5f), new MinMax(3f, 5.5f), 100f, 0.4f), new Cannon(2.2f, new string[6] { "S300,N6,120-240", "S300,N5,134-226", "S300,N7,120-240", "S300,N6,125-245", "S300,N5,139-259", "S300,N7,125-245" }, new string[6] { "S1,S2,S1", "S4,S5,S4", "S1,S2,S3", "S2,S1,S2", "S5,S4,S5", "S4,S5,S6" }, new MinMax(3f, 5.5f)), new Orb(150, 9999, 125, 1.2f, new MinMax(2f, 4f), 6.5f, 1f, true, 1f), new Arms(new MinMax(3f, 5f), "T,M"), new MagnetArms(1f, 3f, -1050f), new TwistyArms(210f, 1.3f, 350f, 0.5f, "350,355,345", 200f, false), new BombBot(16, 250, 6.5f, new MinMax(1f, 2f), 25, 325, 2.3f, 2000), new Heart(225, 35), new HeliHead(800, 0.5f, 0.1f, "250,300,350,400,450,400,350,300"), new BlueGem(656f, 250, new MinMax(150f, 500f), 30, 0.4f, 0, 0.1f, 2.5f, 6, false, new MinMax(0f, 359f)), new RedGem(1214f, 350, new MinMax(100f, 450f), 40, 0.45f, 0, 0f, 2.5f, 8, true, new MinMax(0f, 359f)), new Inventor(0.5f, 1f, new MinMax(8f, 12f), new MinMax(1f, 2f), "R,B", 0, 250, 100, 5f, 4f, 8, 3)));
				list.Add(new State(0.9f, new Pattern[1][] { new Pattern[1] }, States.HeliHead, new Hose(150, 1.5f, 2, new MinMax(0.01f, 0.011f), new MinMax(90f, 170f), 1f), new ShotBot(150, 7, 200, 380, new MinMax(0f, 2f), 4, 1f, new MinMax(1f, 2.5f), new MinMax(3f, 5.5f), 100f, 0.4f), new Cannon(2.2f, new string[6] { "S300,N6,120-240", "S300,N5,134-226", "S300,N7,120-240", "S300,N6,125-245", "S300,N5,139-259", "S300,N7,125-245" }, new string[6] { "S1,S2,S1", "S4,S5,S4", "S1,S2,S3", "S2,S1,S2", "S5,S4,S5", "S4,S5,S6" }, new MinMax(3f, 5.5f)), new Orb(150, 9999, 125, 1.2f, new MinMax(2f, 4f), 6.5f, 1f, true, 1f), new Arms(new MinMax(3f, 5f), "T,M"), new MagnetArms(1f, 3f, -1050f), new TwistyArms(210f, 1.3f, 350f, 0.5f, "350,355,345", 200f, false), new BombBot(16, 250, 6.5f, new MinMax(1f, 2f), 25, 325, 2.3f, 2000), new Heart(225, 35), new HeliHead(800, 0.5f, 0.1f, "250,300,350,400,450,400,350,300"), new BlueGem(656f, 250, new MinMax(150f, 500f), 30, 0.4f, 0, 0.1f, 2.5f, 6, false, new MinMax(0f, 359f)), new RedGem(1214f, 350, new MinMax(100f, 450f), 40, 0.45f, 0, 0f, 2.5f, 8, true, new MinMax(0f, 359f)), new Inventor(0.5f, 1f, new MinMax(8f, 12f), new MinMax(1f, 2f), "R,B", 0, 250, 100, 5f, 4f, 8, 3)));
				list.Add(new State(0.8f, new Pattern[1][] { new Pattern[1] }, States.Inventor, new Hose(150, 1.5f, 2, new MinMax(0.01f, 0.011f), new MinMax(90f, 170f), 1f), new ShotBot(150, 7, 200, 380, new MinMax(0f, 2f), 4, 1f, new MinMax(1f, 2.5f), new MinMax(3f, 5.5f), 100f, 0.4f), new Cannon(2.2f, new string[6] { "S300,N6,120-240", "S300,N5,134-226", "S300,N7,120-240", "S300,N6,125-245", "S300,N5,139-259", "S300,N7,125-245" }, new string[6] { "S1,S2,S1", "S4,S5,S4", "S1,S2,S3", "S2,S1,S2", "S5,S4,S5", "S4,S5,S6" }, new MinMax(3f, 5.5f)), new Orb(150, 9999, 125, 1.2f, new MinMax(2f, 4f), 6.5f, 1f, true, 1f), new Arms(new MinMax(3f, 5f), "T,M"), new MagnetArms(1f, 3f, -1050f), new TwistyArms(210f, 1.3f, 350f, 0.5f, "350,355,345", 200f, false), new BombBot(16, 250, 6.5f, new MinMax(1f, 2f), 25, 325, 2.3f, 2000), new Heart(225, 35), new HeliHead(800, 0.5f, 0.1f, "250,300,350,400,450,400,350,300"), new BlueGem(656f, 250, new MinMax(150f, 500f), 30, 0.4f, 0, 0.1f, 2.5f, 6, false, new MinMax(0f, 359f)), new RedGem(1214f, 350, new MinMax(100f, 450f), 40, 0.45f, 0, 0f, 2.5f, 8, true, new MinMax(0f, 359f)), new Inventor(0.5f, 1f, new MinMax(8f, 12f), new MinMax(1f, 2f), "R,B", 0, 250, 100, 5f, 4f, 8, 3)));
				break;
			case Level.Mode.Hard:
				hp = 1800;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Hose(180, 1.5f, 1, new MinMax(2f, 5f), new MinMax(90f, 170f), 0.8f), new ShotBot(180, 7, 200, 430, new MinMax(0f, 2f), 4, 1f, new MinMax(1f, 2f), new MinMax(3f, 5.5f), 100f, 0.5f), new Cannon(1.8f, new string[6] { "S360,N6,120-240", "S360,N5,134-226", "S370,N7,120-240", "S370,N6,125-245", "S380,N5,139-259", "S380,N7,125-245" }, new string[6] { "S4,S5,S6", "S5,S4,S5", "S2,S1,S2", "S1,S2,S3", "S4,S5,S4", "S4,S5,S6" }, new MinMax(2.5f, 4.5f)), new Orb(180, 9999, 150, 1.1f, new MinMax(2f, 3.5f), 4.5f, 1f, true, 0.5f), new Arms(new MinMax(2f, 4f), "M,T"), new MagnetArms(1f, 3f, -1200f), new TwistyArms(210f, 1f, 380f, 0.5f, "350,355,345", 200f, false), new BombBot(16, 290, 6f, new MinMax(1f, 2f), 25, 375, 2.5f, 2000), new Heart(300, 35), new HeliHead(900, 0.5f, 0.1f, "250,300,350,400,450,400,350,300"), new BlueGem(865f, 250, new MinMax(150f, 525f), 30, 0.3f, 0, 0.1f, 7f, 5, false, new MinMax(0f, 270f)), new RedGem(212f, 350, new MinMax(100f, 450f), 35, 0.42f, 0, 0f, 7f, 8, true, new MinMax(0f, 359f)), new Inventor(0.5f, 1f, new MinMax(8f, 12f), new MinMax(1f, 2f), "R,B", 0, 250, 110, 4.5f, 3.5f, 8, 3)));
				list.Add(new State(0.9f, new Pattern[1][] { new Pattern[0] }, States.HeliHead, new Hose(180, 1.5f, 1, new MinMax(2f, 5f), new MinMax(90f, 170f), 0.8f), new ShotBot(180, 7, 200, 430, new MinMax(0f, 2f), 4, 1f, new MinMax(1f, 2f), new MinMax(3f, 5.5f), 100f, 0.5f), new Cannon(1.8f, new string[6] { "S360,N6,120-240", "S360,N5,134-226", "S370,N7,120-240", "S370,N6,125-245", "S380,N5,139-259", "S380,N7,125-245" }, new string[6] { "S4,S5,S6", "S5,S4,S5", "S2,S1,S2", "S1,S2,S3", "S4,S5,S4", "S4,S5,S6" }, new MinMax(2.5f, 4.5f)), new Orb(180, 9999, 150, 1.1f, new MinMax(2f, 3.5f), 4.5f, 1f, true, 0.5f), new Arms(new MinMax(2f, 4f), "M,T"), new MagnetArms(1f, 3f, -1200f), new TwistyArms(210f, 1f, 380f, 0.5f, "350,355,345", 200f, false), new BombBot(16, 290, 6f, new MinMax(1f, 2f), 25, 375, 2.5f, 2000), new Heart(300, 35), new HeliHead(900, 0.5f, 0.1f, "250,300,350,400,450,400,350,300"), new BlueGem(865f, 250, new MinMax(150f, 525f), 30, 0.3f, 0, 0.1f, 7f, 5, false, new MinMax(0f, 270f)), new RedGem(212f, 350, new MinMax(100f, 450f), 35, 0.42f, 0, 0f, 7f, 8, true, new MinMax(0f, 359f)), new Inventor(0.5f, 1f, new MinMax(8f, 12f), new MinMax(1f, 2f), "R,B", 0, 250, 110, 4.5f, 3.5f, 8, 3)));
				list.Add(new State(0.8f, new Pattern[1][] { new Pattern[0] }, States.Inventor, new Hose(180, 1.5f, 1, new MinMax(2f, 5f), new MinMax(90f, 170f), 0.8f), new ShotBot(180, 7, 200, 430, new MinMax(0f, 2f), 4, 1f, new MinMax(1f, 2f), new MinMax(3f, 5.5f), 100f, 0.5f), new Cannon(1.8f, new string[6] { "S360,N6,120-240", "S360,N5,134-226", "S370,N7,120-240", "S370,N6,125-245", "S380,N5,139-259", "S380,N7,125-245" }, new string[6] { "S4,S5,S6", "S5,S4,S5", "S2,S1,S2", "S1,S2,S3", "S4,S5,S4", "S4,S5,S6" }, new MinMax(2.5f, 4.5f)), new Orb(180, 9999, 150, 1.1f, new MinMax(2f, 3.5f), 4.5f, 1f, true, 0.5f), new Arms(new MinMax(2f, 4f), "M,T"), new MagnetArms(1f, 3f, -1200f), new TwistyArms(210f, 1f, 380f, 0.5f, "350,355,345", 200f, false), new BombBot(16, 290, 6f, new MinMax(1f, 2f), 25, 375, 2.5f, 2000), new Heart(300, 35), new HeliHead(900, 0.5f, 0.1f, "250,300,350,400,450,400,350,300"), new BlueGem(865f, 250, new MinMax(150f, 525f), 30, 0.3f, 0, 0.1f, 7f, 5, false, new MinMax(0f, 270f)), new RedGem(212f, 350, new MinMax(100f, 450f), 35, 0.42f, 0, 0f, 7f, 8, true, new MinMax(0f, 359f)), new Inventor(0.5f, 1f, new MinMax(8f, 12f), new MinMax(1f, 2f), "R,B", 0, 250, 110, 4.5f, 3.5f, 8, 3)));
				break;
			}
			return new Robot(hp, goalTimes, list.ToArray());
		}
	}

	public class SallyStagePlay : AbstractLevelProperties<SallyStagePlay.State, SallyStagePlay.Pattern, SallyStagePlay.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected SallyStagePlay properties { get; private set; }

			public virtual void LevelInit(SallyStagePlay properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			House = 2,
			Angel = 3,
			Final = 4
		}

		public enum Pattern
		{
			Jump = 0,
			Umbrella = 1,
			Kiss = 2,
			Teleport = 3,
			Uninitialized = 4
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Jump jump;

			public readonly DiveKick diveKick;

			public readonly JumpRoll jumpRoll;

			public readonly Shuriken shuriken;

			public readonly Projectile projectile;

			public readonly Umbrella umbrella;

			public readonly Kiss kiss;

			public readonly Teleport teleport;

			public readonly Baby baby;

			public readonly Nun nun;

			public readonly Husband husband;

			public readonly General general;

			public readonly Lightning lightning;

			public readonly Meteor meteor;

			public readonly Tidal tidal;

			public readonly Roses roses;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Jump jump, DiveKick diveKick, JumpRoll jumpRoll, Shuriken shuriken, Projectile projectile, Umbrella umbrella, Kiss kiss, Teleport teleport, Baby baby, Nun nun, Husband husband, General general, Lightning lightning, Meteor meteor, Tidal tidal, Roses roses)
				: base(healthTrigger, patterns, stateName)
			{
				this.jump = jump;
				this.diveKick = diveKick;
				this.jumpRoll = jumpRoll;
				this.shuriken = shuriken;
				this.projectile = projectile;
				this.umbrella = umbrella;
				this.kiss = kiss;
				this.teleport = teleport;
				this.baby = baby;
				this.nun = nun;
				this.husband = husband;
				this.general = general;
				this.lightning = lightning;
				this.meteor = meteor;
				this.tidal = tidal;
				this.roses = roses;
			}
		}

		public class Jump : AbstractLevelPropertyGroup
		{
			public readonly string JumpAttackString;

			public readonly string JumpAttackCountString;

			public readonly MinMax JumpHesitate;

			public readonly float JumpDelay;

			public Jump(string JumpAttackString, string JumpAttackCountString, MinMax JumpHesitate, float JumpDelay)
			{
				this.JumpAttackString = JumpAttackString;
				this.JumpAttackCountString = JumpAttackCountString;
				this.JumpHesitate = JumpHesitate;
				this.JumpDelay = JumpDelay;
			}
		}

		public class DiveKick : AbstractLevelPropertyGroup
		{
			public readonly float DiveSpeed;

			public readonly MinMax DiveAngleRange;

			public readonly MinMax DiveAttackHeight;

			public DiveKick(float DiveSpeed, MinMax DiveAngleRange, MinMax DiveAttackHeight)
			{
				this.DiveSpeed = DiveSpeed;
				this.DiveAngleRange = DiveAngleRange;
				this.DiveAttackHeight = DiveAttackHeight;
			}
		}

		public class JumpRoll : AbstractLevelPropertyGroup
		{
			public readonly float RollJumpVerticalMovement;

			public readonly MinMax RollJumpHorizontalMovement;

			public readonly MinMax JumpHeight;

			public readonly string JumpAttackTypeString;

			public readonly float JumpRollDuration;

			public readonly MinMax RollShotDelayRange;

			public JumpRoll(float RollJumpVerticalMovement, MinMax RollJumpHorizontalMovement, MinMax JumpHeight, string JumpAttackTypeString, float JumpRollDuration, MinMax RollShotDelayRange)
			{
				this.RollJumpVerticalMovement = RollJumpVerticalMovement;
				this.RollJumpHorizontalMovement = RollJumpHorizontalMovement;
				this.JumpHeight = JumpHeight;
				this.JumpAttackTypeString = JumpAttackTypeString;
				this.JumpRollDuration = JumpRollDuration;
				this.RollShotDelayRange = RollShotDelayRange;
			}
		}

		public class Shuriken : AbstractLevelPropertyGroup
		{
			public readonly float InitialMovementSpeed;

			public readonly float ArcOneGravity;

			public readonly float ArcOneVerticalVelocity;

			public readonly float ArcOneHorizontalVelocity;

			public readonly float ArcTwoGravity;

			public readonly float ArcTwoVerticalVelocity;

			public readonly float ArcTwoHorizontalVelocity;

			public readonly int NumberOfChildSpawns;

			public Shuriken(float InitialMovementSpeed, float ArcOneGravity, float ArcOneVerticalVelocity, float ArcOneHorizontalVelocity, float ArcTwoGravity, float ArcTwoVerticalVelocity, float ArcTwoHorizontalVelocity, int NumberOfChildSpawns)
			{
				this.InitialMovementSpeed = InitialMovementSpeed;
				this.ArcOneGravity = ArcOneGravity;
				this.ArcOneVerticalVelocity = ArcOneVerticalVelocity;
				this.ArcOneHorizontalVelocity = ArcOneHorizontalVelocity;
				this.ArcTwoGravity = ArcTwoGravity;
				this.ArcTwoVerticalVelocity = ArcTwoVerticalVelocity;
				this.ArcTwoHorizontalVelocity = ArcTwoHorizontalVelocity;
				this.NumberOfChildSpawns = NumberOfChildSpawns;
			}
		}

		public class Projectile : AbstractLevelPropertyGroup
		{
			public readonly float projectileSpeed;

			public readonly float groundDuration;

			public readonly float groundSize;

			public Projectile(float projectileSpeed, float groundDuration, float groundSize)
			{
				this.projectileSpeed = projectileSpeed;
				this.groundDuration = groundDuration;
				this.groundSize = groundSize;
			}
		}

		public class Umbrella : AbstractLevelPropertyGroup
		{
			public readonly float initialAttackDelay;

			public readonly float objectSpeed;

			public readonly float objectDropSpeed;

			public readonly int objectCount;

			public readonly float objectDelay;

			public readonly float hesitate;

			public readonly float homingMaxSpeed;

			public readonly float homingAcceleration;

			public readonly float homingBounceRatio;

			public readonly float homingUntilSwitchPlayer;

			public Umbrella(float initialAttackDelay, float objectSpeed, float objectDropSpeed, int objectCount, float objectDelay, float hesitate, float homingMaxSpeed, float homingAcceleration, float homingBounceRatio, float homingUntilSwitchPlayer)
			{
				this.initialAttackDelay = initialAttackDelay;
				this.objectSpeed = objectSpeed;
				this.objectDropSpeed = objectDropSpeed;
				this.objectCount = objectCount;
				this.objectDelay = objectDelay;
				this.hesitate = hesitate;
				this.homingMaxSpeed = homingMaxSpeed;
				this.homingAcceleration = homingAcceleration;
				this.homingBounceRatio = homingBounceRatio;
				this.homingUntilSwitchPlayer = homingUntilSwitchPlayer;
			}
		}

		public class Kiss : AbstractLevelPropertyGroup
		{
			public readonly float heartSpeed;

			public readonly string heartType;

			public readonly float sineWaveSpeed;

			public readonly float sineWaveStrength;

			public readonly float hesitate;

			public Kiss(float heartSpeed, string heartType, float sineWaveSpeed, float sineWaveStrength, float hesitate)
			{
				this.heartSpeed = heartSpeed;
				this.heartType = heartType;
				this.sineWaveSpeed = sineWaveSpeed;
				this.sineWaveStrength = sineWaveStrength;
				this.hesitate = hesitate;
			}
		}

		public class Teleport : AbstractLevelPropertyGroup
		{
			public readonly string appearOffsetString;

			public readonly MinMax fallingSpeed;

			public readonly float acceleration;

			public readonly float hesitate;

			public readonly float offScreenDelay;

			public readonly float sawAttackDuration;

			public Teleport(string appearOffsetString, MinMax fallingSpeed, float acceleration, float hesitate, float offScreenDelay, float sawAttackDuration)
			{
				this.appearOffsetString = appearOffsetString;
				this.fallingSpeed = fallingSpeed;
				this.acceleration = acceleration;
				this.hesitate = hesitate;
				this.offScreenDelay = offScreenDelay;
				this.sawAttackDuration = sawAttackDuration;
			}
		}

		public class Baby : AbstractLevelPropertyGroup
		{
			public readonly float bottleSpeed;

			public readonly float attackDelay;

			public readonly int HP;

			public readonly MinMax reappearDelayRange;

			public readonly string[] appearPosition;

			public readonly float hesitate;

			public Baby(float bottleSpeed, float attackDelay, int HP, MinMax reappearDelayRange, string[] appearPosition, float hesitate)
			{
				this.bottleSpeed = bottleSpeed;
				this.attackDelay = attackDelay;
				this.HP = HP;
				this.reappearDelayRange = reappearDelayRange;
				this.appearPosition = appearPosition;
				this.hesitate = hesitate;
			}
		}

		public class Nun : AbstractLevelPropertyGroup
		{
			public readonly float rulerSpeed;

			public readonly float attackDelay;

			public readonly int HP;

			public readonly MinMax reappearDelayRange;

			public readonly string[] appearPosition;

			public readonly float hesitate;

			public Nun(float rulerSpeed, float attackDelay, int HP, MinMax reappearDelayRange, string[] appearPosition, float hesitate)
			{
				this.rulerSpeed = rulerSpeed;
				this.attackDelay = attackDelay;
				this.HP = HP;
				this.reappearDelayRange = reappearDelayRange;
				this.appearPosition = appearPosition;
				this.hesitate = hesitate;
			}
		}

		public class Husband : AbstractLevelPropertyGroup
		{
			public readonly float deityHP;

			public readonly MinMax shotDelayRange;

			public readonly float shotSpeed;

			public readonly float shotScale;

			public Husband(float deityHP, MinMax shotDelayRange, float shotSpeed, float shotScale)
			{
				this.deityHP = deityHP;
				this.shotDelayRange = shotDelayRange;
				this.shotSpeed = shotSpeed;
				this.shotScale = shotScale;
			}
		}

		public class General : AbstractLevelPropertyGroup
		{
			public readonly string[] attackString;

			public readonly MinMax attackDelayRange;

			public readonly float finalMovementSpeed;

			public readonly float cupidDropMaxY;

			public readonly float cupidMoveSpeed;

			public General(string[] attackString, MinMax attackDelayRange, float finalMovementSpeed, float cupidDropMaxY, float cupidMoveSpeed)
			{
				this.attackString = attackString;
				this.attackDelayRange = attackDelayRange;
				this.finalMovementSpeed = finalMovementSpeed;
				this.cupidDropMaxY = cupidDropMaxY;
				this.cupidMoveSpeed = cupidMoveSpeed;
			}
		}

		public class Lightning : AbstractLevelPropertyGroup
		{
			public readonly float lightningSpeed;

			public readonly string lightningAngleString;

			public readonly MinMax lightningDirectAimRange;

			public readonly string lightningShotCount;

			public readonly MinMax lightningDelayRange;

			public readonly string lightningSpawnString;

			public Lightning(float lightningSpeed, string lightningAngleString, MinMax lightningDirectAimRange, string lightningShotCount, MinMax lightningDelayRange, string lightningSpawnString)
			{
				this.lightningSpeed = lightningSpeed;
				this.lightningAngleString = lightningAngleString;
				this.lightningDirectAimRange = lightningDirectAimRange;
				this.lightningShotCount = lightningShotCount;
				this.lightningDelayRange = lightningDelayRange;
				this.lightningSpawnString = lightningSpawnString;
			}
		}

		public class Meteor : AbstractLevelPropertyGroup
		{
			public readonly float meteorSpeed;

			public readonly int meteorHP;

			public readonly float hookSpeed;

			public readonly float hookMaxHeight;

			public readonly float hookRevealExitDelay;

			public readonly float hookParryExitDelay;

			public readonly float meteorSize;

			public readonly string meteorSpawnString;

			public Meteor(float meteorSpeed, int meteorHP, float hookSpeed, float hookMaxHeight, float hookRevealExitDelay, float hookParryExitDelay, float meteorSize, string meteorSpawnString)
			{
				this.meteorSpeed = meteorSpeed;
				this.meteorHP = meteorHP;
				this.hookSpeed = hookSpeed;
				this.hookMaxHeight = hookMaxHeight;
				this.hookRevealExitDelay = hookRevealExitDelay;
				this.hookParryExitDelay = hookParryExitDelay;
				this.meteorSize = meteorSize;
				this.meteorSpawnString = meteorSpawnString;
			}
		}

		public class Tidal : AbstractLevelPropertyGroup
		{
			public readonly float tidalSpeed;

			public readonly float tidalSize;

			public readonly float tidalHesitate;

			public Tidal(float tidalSpeed, float tidalSize, float tidalHesitate)
			{
				this.tidalSpeed = tidalSpeed;
				this.tidalSize = tidalSize;
				this.tidalHesitate = tidalHesitate;
			}
		}

		public class Roses : AbstractLevelPropertyGroup
		{
			public readonly MinMax fallSpeed;

			public readonly float fallAcceleration;

			public readonly float groundDuration;

			public readonly string[] spawnString;

			public readonly MinMax spawnDelayRange;

			public readonly MinMax playerAimRange;

			public Roses(MinMax fallSpeed, float fallAcceleration, float groundDuration, string[] spawnString, MinMax spawnDelayRange, MinMax playerAimRange)
			{
				this.fallSpeed = fallSpeed;
				this.fallAcceleration = fallAcceleration;
				this.groundDuration = groundDuration;
				this.spawnString = spawnString;
				this.spawnDelayRange = spawnDelayRange;
				this.playerAimRange = playerAimRange;
			}
		}

		public SallyStagePlay(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1100f;
				timeline.events.Add(new Level.Timeline.Event("House", 0.65f));
				timeline.events.Add(new Level.Timeline.Event("Angel", 0.3f));
				break;
			case Level.Mode.Normal:
				timeline.health = 1400f;
				timeline.events.Add(new Level.Timeline.Event("House", 0.72f));
				timeline.events.Add(new Level.Timeline.Event("Angel", 0.43f));
				timeline.events.Add(new Level.Timeline.Event("Final", 0.14f));
				break;
			case Level.Mode.Hard:
				timeline.health = 1700f;
				timeline.events.Add(new Level.Timeline.Event("House", 0.72f));
				timeline.events.Add(new Level.Timeline.Event("Angel", 0.43f));
				timeline.events.Add(new Level.Timeline.Event("Final", 0.14f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "J":
				return Pattern.Jump;
			case "U":
				return Pattern.Umbrella;
			case "K":
				return Pattern.Kiss;
			case "T":
				return Pattern.Teleport;
			default:
				Debug.LogError("Pattern SallyStagePlay.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static SallyStagePlay GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[10]
				{
					Pattern.Jump,
					Pattern.Kiss,
					Pattern.Teleport,
					Pattern.Jump,
					Pattern.Kiss,
					Pattern.Jump,
					Pattern.Teleport,
					Pattern.Kiss,
					Pattern.Jump,
					Pattern.Teleport
				} }, States.Main, new Jump("1,1,2,1,1,1,2", "1,3,2,3,1,2,3,2,2", new MinMax(0f, 0.5f), 0.3f), new DiveKick(500f, new MinMax(25f, 35f), new MinMax(375f, 415f)), new JumpRoll(100f, new MinMax(50f, 100f), new MinMax(250f, 300f), "B", 0.3f, new MinMax(0f, 0.3f)), new Shuriken(850f, 20f, 625f, 150f, 20f, 625f, 125f, 2), new Projectile(650f, 5f, 2f), new Umbrella(0.5f, 400f, 450f, 1, 1f, 1f, 775f, 1400f, 0.55f, 10f), new Kiss(150f, "P,P", 4.8f, 150f, 1f), new Teleport("0,100,-100,0,50,0,-50", new MinMax(200f, 500f), 3f, 0.1f, 0.2f, 1f), new Baby(380f, 1f, 20, new MinMax(2.3f, 3.2f), new string[4] { "1,5,4,6,2,9,3,7,8,1,7,4,6,2,9,3,5,8", "3,9,7,1,4,2,5,8,6,3,2,7,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,6,9,3,8,7,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.1f), new Nun(380f, 0.7f, 20, new MinMax(2.3f, 3.2f), new string[4] { "1,5,4,2,3,1,4,2,3,5", "3,1,4,2,5,3,2,1,4,5", "5,3,1,4,2,5,3,4,2,1", "1,3,5,2,4,1,5,3,2,4" }, 0.3f), new Husband(10f, new MinMax(3f, 5f), 450f, 1f), new General(new string[1] { "M,T,M,T" }, new MinMax(1f, 1.5f), 3.5f, 400f, 80f), new Lightning(550f, "140,190,165,150,130,180,145,165,140,175,155,160", new MinMax(5f, 8f), "3,2,2,2,3,3,2,3,2,3,3,3,2,3", new MinMax(0.7f, 1.1f), "1000,560,320,450,605,877,344,966,606,1050,1200,350,750,1100,400,844,590"), new Meteor(455f, 26, 100f, 200f, 1f, 2f, 1f, "1000,750,950,800,1100,850,900,1200,950,1150"), new Tidal(500f, 1.2f, 0.1f), new Roses(new MinMax(300f, 550f), 3f, 7f, new string[1] { "100,500,200,1100,800,600,400,1000,700,100,900,300,600,1000" }, new MinMax(2.5f, 4f), new MinMax(4f, 7f))));
				list.Add(new State(0.65f, new Pattern[1][] { new Pattern[4]
				{
					Pattern.Umbrella,
					Pattern.Jump,
					Pattern.Umbrella,
					Pattern.Teleport
				} }, States.House, new Jump("1,1", "3,2,2,1,2,2,3,1,2,3,2", new MinMax(0f, 0.5f), 0.3f), new DiveKick(500f, new MinMax(25f, 35f), new MinMax(375f, 415f)), new JumpRoll(100f, new MinMax(50f, 100f), new MinMax(200f, 300f), "B", 0.3f, new MinMax(0f, 0.3f)), new Shuriken(850f, 20f, 625f, 150f, 20f, 625f, 125f, 2), new Projectile(650f, 5f, 2f), new Umbrella(0.5f, 400f, 450f, 1, 1f, 1f, 775f, 1400f, 0.55f, 10f), new Kiss(150f, "P,P", 4.8f, 150f, 1f), new Teleport("0,100,-100,0,50,0,-50", new MinMax(200f, 500f), 3f, 0.1f, 0.2f, 1f), new Baby(380f, 1f, 20, new MinMax(2.3f, 3.2f), new string[4] { "1,5,4,6,2,9,3,7,8,1,7,4,6,2,9,3,5,8", "3,9,7,1,4,2,5,8,6,3,2,7,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,6,9,3,8,7,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.1f), new Nun(380f, 0.7f, 20, new MinMax(2.3f, 3.2f), new string[4] { "1,5,4,2,3,1,4,2,3,5", "3,1,4,2,5,3,2,1,4,5", "5,3,1,4,2,5,3,4,2,1", "1,3,5,2,4,1,5,3,2,4" }, 0.3f), new Husband(10f, new MinMax(3f, 5f), 450f, 1f), new General(new string[1] { "M,T,M,T" }, new MinMax(1f, 1.5f), 3.5f, 400f, 80f), new Lightning(550f, "140,190,165,150,130,180,145,165,140,175,155,160", new MinMax(5f, 8f), "3,2,2,2,3,3,2,3,2,3,3,3,2,3", new MinMax(0.7f, 1.1f), "1000,560,320,450,605,877,344,966,606,1050,1200,350,750,1100,400,844,590"), new Meteor(455f, 26, 100f, 200f, 1f, 2f, 1f, "1000,750,950,800,1100,850,900,1200,950,1150"), new Tidal(500f, 1.2f, 0.1f), new Roses(new MinMax(300f, 550f), 3f, 7f, new string[1] { "100,500,200,1100,800,600,400,1000,700,100,900,300,600,1000" }, new MinMax(2.5f, 4f), new MinMax(4f, 7f))));
				list.Add(new State(0.3f, new Pattern[1][] { new Pattern[0] }, States.Angel, new Jump("1,1", "3,2,2,1,2,2,3,1,2,3,2", new MinMax(0f, 0.5f), 0.3f), new DiveKick(500f, new MinMax(25f, 35f), new MinMax(375f, 415f)), new JumpRoll(100f, new MinMax(50f, 100f), new MinMax(200f, 300f), "B", 0.3f, new MinMax(0f, 0.3f)), new Shuriken(850f, 20f, 625f, 150f, 20f, 625f, 125f, 2), new Projectile(650f, 5f, 2f), new Umbrella(0.5f, 400f, 450f, 1, 1f, 1f, 775f, 1400f, 0.55f, 10f), new Kiss(150f, "P,P", 4.8f, 150f, 1f), new Teleport("0,100,-100,0,50,0,-50", new MinMax(200f, 500f), 3f, 0.1f, 0.2f, 1f), new Baby(380f, 1f, 20, new MinMax(2.3f, 3.2f), new string[4] { "1,5,4,6,2,9,3,7,8,1,7,4,6,2,9,3,5,8", "3,9,7,1,4,2,5,8,6,3,2,7,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,6,9,3,8,7,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.1f), new Nun(380f, 0.7f, 20, new MinMax(2.3f, 3.2f), new string[4] { "1,5,4,2,3,1,4,2,3,5", "3,1,4,2,5,3,2,1,4,5", "5,3,1,4,2,5,3,4,2,1", "1,3,5,2,4,1,5,3,2,4" }, 0.3f), new Husband(10f, new MinMax(3f, 5f), 450f, 1f), new General(new string[1] { "M,T,M,T" }, new MinMax(1f, 1.5f), 3.5f, 400f, 80f), new Lightning(550f, "140,190,165,150,130,180,145,165,140,175,155,160", new MinMax(5f, 8f), "3,2,2,2,3,3,2,3,2,3,3,3,2,3", new MinMax(0.7f, 1.1f), "1000,560,320,450,605,877,344,966,606,1050,1200,350,750,1100,400,844,590"), new Meteor(455f, 26, 100f, 200f, 1f, 2f, 1f, "1000,750,950,800,1100,850,900,1200,950,1150"), new Tidal(500f, 1.2f, 0.1f), new Roses(new MinMax(300f, 550f), 3f, 7f, new string[1] { "100,500,200,1100,800,600,400,1000,700,100,900,300,600,1000" }, new MinMax(2.5f, 4f), new MinMax(4f, 7f))));
				break;
			case Level.Mode.Normal:
				hp = 1400;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[7]
				{
					Pattern.Jump,
					Pattern.Kiss,
					Pattern.Teleport,
					Pattern.Jump,
					Pattern.Kiss,
					Pattern.Jump,
					Pattern.Teleport
				} }, States.Main, new Jump("1,1,2,1,2,1,1,1,2", "1,3,2,3,1,2,3,2,2,3", new MinMax(0f, 0.5f), 0.3f), new DiveKick(600f, new MinMax(25f, 35f), new MinMax(360f, 440f)), new JumpRoll(100f, new MinMax(50f, 100f), new MinMax(200f, 300f), "B", 0.3f, new MinMax(0f, 0.3f)), new Shuriken(800f, 20f, 675f, 175f, 20f, 625f, 125f, 1), new Projectile(750f, 6f, 2f), new Umbrella(0.5f, 430f, 265f, 2, 1.35f, 2f, 685f, 1200f, 0.6f, 10f), new Kiss(150f, "P,P", 5f, 175f, 1f), new Teleport("0,100,-100,0,200,-200,50,-200,0,150", new MinMax(300f, 600f), 5f, 0.1f, 0.2f, 1f), new Baby(380f, 1.5f, 20, new MinMax(2f, 3f), new string[4] { "1,5,4,6,2,9,3,7,8,1,7,4,6,2,9,3,5,8", "3,9,7,1,4,2,5,8,6,3,2,7,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,6,9,3,8,7,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.1f), new Nun(410f, 0.7f, 20, new MinMax(2f, 3f), new string[4] { "1,5,4,2,9,3,8,1,7,4,6,2,3,5", "3,7,1,4,2,5,3,2,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,3,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.3f), new Husband(100f, new MinMax(3f, 5f), 450f, 1f), new General(new string[2] { "L,L,M,L,T,L,M,T,L,L,M,T", "L,M,T,L,M,L,T,L,M,T,L,L,M,L,T" }, new MinMax(1.3f, 2f), 3.5f, 400f, 80f), new Lightning(650f, "140,190,165,150,130,180,145,165,140,175,155,160", new MinMax(4f, 6f), "3,2,4,2,3,4,2,3,4,3,3,4,4,3", new MinMax(0.5f, 0.9f), "1000,560,320,450,605,877,344,966,606,1050,1200,350,750,1100,400,844,590"), new Meteor(405f, 35, 100f, 200f, 1f, 2f, 1f, "1000,750,950,800,1100,850,750,900,1200,950,750,900,1150"), new Tidal(400f, 1.2f, 0.1f), new Roses(new MinMax(300f, 550f), 2f, 0f, new string[1] { "100,500,200,1100,800,600,400,1000,700,100,900,300,600,1000" }, new MinMax(3f, 4.5f), new MinMax(4f, 7f))));
				list.Add(new State(0.72f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Umbrella,
					Pattern.Jump
				} }, States.House, new Jump("1,1,2,1,1,2,1,1,1,2", "1,3,2,3,1,2,2,3,1,2,3,2", new MinMax(0f, 0.5f), 0.3f), new DiveKick(600f, new MinMax(25f, 35f), new MinMax(360f, 440f)), new JumpRoll(100f, new MinMax(50f, 100f), new MinMax(200f, 300f), "B,B", 0.3f, new MinMax(0f, 0.3f)), new Shuriken(800f, 20f, 675f, 175f, 20f, 625f, 125f, 1), new Projectile(750f, 6f, 2f), new Umbrella(0.5f, 430f, 265f, 2, 1.35f, 2f, 685f, 1200f, 0.6f, 10f), new Kiss(150f, "P,P", 5f, 175f, 1f), new Teleport("0,100,-100,0,200,-200,50,-200,0,150", new MinMax(300f, 600f), 5f, 0.1f, 0.2f, 1f), new Baby(380f, 1.5f, 20, new MinMax(2f, 3f), new string[4] { "1,5,4,6,2,9,3,7,8,1,7,4,6,2,9,3,5,8", "3,9,7,1,4,2,5,8,6,3,2,7,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,6,9,3,8,7,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.1f), new Nun(410f, 0.7f, 20, new MinMax(2f, 3f), new string[4] { "1,5,4,2,9,3,8,1,7,4,6,2,3,5", "3,7,1,4,2,5,3,2,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,3,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.3f), new Husband(100f, new MinMax(3f, 5f), 450f, 1f), new General(new string[2] { "L,L,M,L,T,L,M,T,L,L,M,T", "L,M,T,L,M,L,T,L,M,T,L,L,M,L,T" }, new MinMax(1.3f, 2f), 3.5f, 400f, 80f), new Lightning(650f, "140,190,165,150,130,180,145,165,140,175,155,160", new MinMax(4f, 6f), "3,2,4,2,3,4,2,3,4,3,3,4,4,3", new MinMax(0.5f, 0.9f), "1000,560,320,450,605,877,344,966,606,1050,1200,350,750,1100,400,844,590"), new Meteor(405f, 35, 100f, 200f, 1f, 2f, 1f, "1000,750,950,800,1100,850,750,900,1200,950,750,900,1150"), new Tidal(400f, 1.2f, 0.1f), new Roses(new MinMax(300f, 550f), 2f, 0f, new string[1] { "100,500,200,1100,800,600,400,1000,700,100,900,300,600,1000" }, new MinMax(3f, 4.5f), new MinMax(4f, 7f))));
				list.Add(new State(0.43f, new Pattern[1][] { new Pattern[0] }, States.Angel, new Jump("1,1,2,1,1,2,1,1,1,2", "1,3,2,3,1,2,2,3,1,2,3,2", new MinMax(0f, 0.5f), 0.3f), new DiveKick(600f, new MinMax(25f, 35f), new MinMax(360f, 440f)), new JumpRoll(100f, new MinMax(50f, 100f), new MinMax(200f, 300f), "B,B", 0.3f, new MinMax(0f, 0.3f)), new Shuriken(800f, 20f, 675f, 175f, 20f, 625f, 125f, 1), new Projectile(750f, 6f, 2f), new Umbrella(0.5f, 430f, 265f, 2, 1.35f, 2f, 685f, 1200f, 0.6f, 10f), new Kiss(150f, "P,P", 5f, 175f, 1f), new Teleport("0,100,-100,0,200,-200,50,-200,0,150", new MinMax(300f, 600f), 5f, 0.1f, 0.2f, 1f), new Baby(380f, 1.5f, 20, new MinMax(2f, 3f), new string[4] { "1,5,4,6,2,9,3,7,8,1,7,4,6,2,9,3,5,8", "3,9,7,1,4,2,5,8,6,3,2,7,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,6,9,3,8,7,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.1f), new Nun(410f, 0.7f, 20, new MinMax(2f, 3f), new string[4] { "1,5,4,2,9,3,8,1,7,4,6,2,3,5", "3,7,1,4,2,5,3,2,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,3,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.3f), new Husband(100f, new MinMax(3f, 5f), 450f, 1f), new General(new string[2] { "L,L,M,L,T,L,M,T,L,L,M,T", "L,M,T,L,M,L,T,L,M,T,L,L,M,L,T" }, new MinMax(1.3f, 2f), 3.5f, 400f, 80f), new Lightning(650f, "140,190,165,150,130,180,145,165,140,175,155,160", new MinMax(4f, 6f), "3,2,4,2,3,4,2,3,4,3,3,4,4,3", new MinMax(0.5f, 0.9f), "1000,560,320,450,605,877,344,966,606,1050,1200,350,750,1100,400,844,590"), new Meteor(405f, 35, 100f, 200f, 1f, 2f, 1f, "1000,750,950,800,1100,850,750,900,1200,950,750,900,1150"), new Tidal(400f, 1.2f, 0.1f), new Roses(new MinMax(300f, 550f), 2f, 0f, new string[1] { "100,500,200,1100,800,600,400,1000,700,100,900,300,600,1000" }, new MinMax(3f, 4.5f), new MinMax(4f, 7f))));
				list.Add(new State(0.14f, new Pattern[1][] { new Pattern[0] }, States.Final, new Jump("1,1,2,1,1,2,1,1,1,2", "1,3,2,3,1,2,2,3,1,2,3,2", new MinMax(0f, 0.5f), 0.3f), new DiveKick(600f, new MinMax(25f, 35f), new MinMax(360f, 440f)), new JumpRoll(100f, new MinMax(50f, 100f), new MinMax(200f, 300f), "B,B", 0.3f, new MinMax(0f, 0.3f)), new Shuriken(800f, 20f, 675f, 175f, 20f, 625f, 125f, 1), new Projectile(750f, 6f, 2f), new Umbrella(0.5f, 430f, 265f, 2, 1.35f, 2f, 685f, 1200f, 0.6f, 10f), new Kiss(150f, "P,P", 5f, 175f, 1f), new Teleport("0,100,-100,0,200,-200,50,-200,0,150", new MinMax(300f, 600f), 5f, 0.1f, 0.2f, 1f), new Baby(380f, 1.5f, 20, new MinMax(2f, 3f), new string[4] { "1,5,4,6,2,9,3,7,8,1,7,4,6,2,9,3,5,8", "3,9,7,1,4,2,5,8,6,3,2,7,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,6,9,3,8,7,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.1f), new Nun(410f, 0.7f, 20, new MinMax(2f, 3f), new string[4] { "1,5,4,2,9,3,8,1,7,4,6,2,3,5", "3,7,1,4,2,5,3,2,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,3,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.3f), new Husband(100f, new MinMax(3f, 5f), 450f, 1f), new General(new string[2] { "L,L,M,L,T,L,M,T,L,L,M,T", "L,M,T,L,M,L,T,L,M,T,L,L,M,L,T" }, new MinMax(1.3f, 2f), 3.5f, 400f, 80f), new Lightning(650f, "140,190,165,150,130,180,145,165,140,175,155,160", new MinMax(4f, 6f), "3,2,4,2,3,4,2,3,4,3,3,4,4,3", new MinMax(0.5f, 0.9f), "1000,560,320,450,605,877,344,966,606,1050,1200,350,750,1100,400,844,590"), new Meteor(405f, 35, 100f, 200f, 1f, 2f, 1f, "1000,750,950,800,1100,850,750,900,1200,950,750,900,1150"), new Tidal(400f, 1.2f, 0.1f), new Roses(new MinMax(300f, 550f), 2f, 0f, new string[1] { "100,500,200,1100,800,600,400,1000,700,100,900,300,600,1000" }, new MinMax(3f, 4.5f), new MinMax(4f, 7f))));
				break;
			case Level.Mode.Hard:
				hp = 1700;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[7]
				{
					Pattern.Jump,
					Pattern.Kiss,
					Pattern.Teleport,
					Pattern.Jump,
					Pattern.Kiss,
					Pattern.Jump,
					Pattern.Teleport
				} }, States.Main, new Jump("1,1,2,1,2,1,1,1,2", "1,3,2,3,1,2,3,2,2,3", new MinMax(0f, 0.5f), 0.3f), new DiveKick(700f, new MinMax(25f, 35f), new MinMax(350f, 430f)), new JumpRoll(100f, new MinMax(50f, 100f), new MinMax(200f, 300f), "B", 0.3f, new MinMax(0f, 0.3f)), new Shuriken(850f, 20f, 625f, 150f, 20f, 625f, 125f, 2), new Projectile(750f, 7f, 2f), new Umbrella(0.5f, 430f, 265f, 2, 1f, 2f, 745f, 1300f, 0.5f, 10f), new Kiss(170f, "P,P", 5.6f, 175f, 0.5f), new Teleport("0,100,-100,0,200,-200,50,-200,0,150", new MinMax(300f, 650f), 6.5f, 0.1f, 0.2f, 1f), new Baby(410f, 1f, 20, new MinMax(1.3f, 2.1f), new string[4] { "1,5,4,6,2,9,3,7,8,1,7,4,6,2,9,3,5,8", "3,9,7,1,4,2,5,8,6,3,2,7,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,6,9,3,8,7,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.1f), new Nun(410f, 0.7f, 20, new MinMax(1.3f, 2.1f), new string[4] { "1,5,4,2,9,3,8,1,7,4,6,2,3,5", "3,7,1,4,2,5,3,2,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,3,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.3f), new Husband(10f, new MinMax(2f, 4f), 475f, 1f), new General(new string[2] { "L,L,M,L,T,L,M,T,L,L,M,T", "L,M,T,L,M,L,T,L,M,T,L,L,M,L,T" }, new MinMax(0.5f, 1.2f), 3.5f, 400f, 80f), new Lightning(650f, "140,190,165,150,130,180,145,165,140,175,155,160", new MinMax(4f, 6f), "3,2,4,2,3,4,2,3,4,3,3,4,4,3", new MinMax(0.5f, 0.9f), "1100,540,320,1170,655,827,344,916,626,1050,1200,360,710,1100,440,844,570,1150,377,687,954,515,905"), new Meteor(455f, 35, 100f, 200f, 1f, 2f, 1f, "1000,750,950,800,1100,850,750,900,1200,950,750,900,1150"), new Tidal(450f, 1.2f, 0.1f), new Roses(new MinMax(300f, 550f), 2f, 0f, new string[1] { "100,500,200,1100,800,600,400,1000,700,100,900,300,600,1000" }, new MinMax(2.5f, 3.5f), new MinMax(4f, 7f))));
				list.Add(new State(0.72f, new Pattern[1][] { new Pattern[2]
				{
					Pattern.Umbrella,
					Pattern.Jump
				} }, States.House, new Jump("1,1,2,1,1,2,1,1,1,2", "1,3,2,3,1,2,2,3,1,2,3,2", new MinMax(0f, 0.5f), 0.3f), new DiveKick(700f, new MinMax(25f, 35f), new MinMax(350f, 430f)), new JumpRoll(100f, new MinMax(50f, 100f), new MinMax(200f, 300f), "B", 0.3f, new MinMax(0f, 0.3f)), new Shuriken(850f, 20f, 625f, 150f, 20f, 625f, 125f, 2), new Projectile(750f, 6f, 2f), new Umbrella(0.5f, 430f, 265f, 2, 1f, 2f, 745f, 1300f, 0.5f, 10f), new Kiss(170f, "P,P", 5.6f, 175f, 0.5f), new Teleport("0,100,-100,0,200,-200,50,-200,0,150", new MinMax(300f, 650f), 6.5f, 0.1f, 0.2f, 1f), new Baby(410f, 1f, 20, new MinMax(1.3f, 2.1f), new string[4] { "1,5,4,6,2,9,3,7,8,1,7,4,6,2,9,3,5,8", "3,9,7,1,4,2,5,8,6,3,2,7,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,6,9,3,8,7,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.1f), new Nun(410f, 0.7f, 20, new MinMax(1.3f, 2.1f), new string[4] { "1,5,4,2,9,3,8,1,7,4,6,2,3,5", "3,7,1,4,2,5,3,2,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,3,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.3f), new Husband(10f, new MinMax(2f, 4f), 475f, 1f), new General(new string[2] { "L,L,M,L,T,L,M,T,L,L,M,T", "L,M,T,L,M,L,T,L,M,T,L,L,M,L,T" }, new MinMax(0.5f, 1.2f), 3.5f, 400f, 80f), new Lightning(650f, "140,190,165,150,130,180,145,165,140,175,155,160", new MinMax(4f, 6f), "3,2,4,2,3,4,2,3,4,3,3,4,4,3", new MinMax(0.5f, 0.9f), "1100,540,320,1170,655,827,344,916,626,1050,1200,360,710,1100,440,844,570,1150,377,687,954,515,905"), new Meteor(455f, 35, 100f, 200f, 1f, 2f, 1f, "1000,750,950,800,1100,850,750,900,1200,950,750,900,1150"), new Tidal(450f, 1.2f, 0.1f), new Roses(new MinMax(300f, 550f), 2f, 0f, new string[1] { "100,500,200,1100,800,600,400,1000,700,100,900,300,600,1000" }, new MinMax(2.5f, 3.5f), new MinMax(4f, 7f))));
				list.Add(new State(0.43f, new Pattern[1][] { new Pattern[0] }, States.Angel, new Jump("1,1,2,1,1,2,1,1,1,2", "1,3,2,3,1,2,2,3,1,2,3,2", new MinMax(0f, 0.5f), 0.3f), new DiveKick(700f, new MinMax(25f, 35f), new MinMax(350f, 430f)), new JumpRoll(100f, new MinMax(50f, 100f), new MinMax(200f, 300f), "B", 0.3f, new MinMax(0f, 0.3f)), new Shuriken(850f, 20f, 625f, 150f, 20f, 625f, 125f, 2), new Projectile(750f, 6f, 2f), new Umbrella(0.5f, 430f, 265f, 2, 1f, 2f, 745f, 1300f, 0.5f, 10f), new Kiss(170f, "P,P", 5.6f, 175f, 0.5f), new Teleport("0,100,-100,0,200,-200,50,-200,0,150", new MinMax(300f, 650f), 6.5f, 0.1f, 0.2f, 1f), new Baby(410f, 1f, 20, new MinMax(1.3f, 2.1f), new string[4] { "1,5,4,6,2,9,3,7,8,1,7,4,6,2,9,3,5,8", "3,9,7,1,4,2,5,8,6,3,2,7,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,6,9,3,8,7,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.1f), new Nun(410f, 0.7f, 20, new MinMax(1.3f, 2.1f), new string[4] { "1,5,4,2,9,3,8,1,7,4,6,2,3,5", "3,7,1,4,2,5,3,2,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,3,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.3f), new Husband(10f, new MinMax(2f, 4f), 475f, 1f), new General(new string[2] { "L,L,M,L,T,L,M,T,L,L,M,T", "L,M,T,L,M,L,T,L,M,T,L,L,M,L,T" }, new MinMax(0.5f, 1.2f), 3.5f, 400f, 80f), new Lightning(650f, "140,190,165,150,130,180,145,165,140,175,155,160", new MinMax(4f, 6f), "3,2,4,2,3,4,2,3,4,3,3,4,4,3", new MinMax(0.5f, 0.9f), "1100,540,320,1170,655,827,344,916,626,1050,1200,360,710,1100,440,844,570,1150,377,687,954,515,905"), new Meteor(455f, 35, 100f, 200f, 1f, 2f, 1f, "1000,750,950,800,1100,850,750,900,1200,950,750,900,1150"), new Tidal(450f, 1.2f, 0.1f), new Roses(new MinMax(300f, 550f), 2f, 0f, new string[1] { "100,500,200,1100,800,600,400,1000,700,100,900,300,600,1000" }, new MinMax(2.5f, 3.5f), new MinMax(4f, 7f))));
				list.Add(new State(0.14f, new Pattern[1][] { new Pattern[0] }, States.Final, new Jump("1,1,2,1,1,2,1,1,1,2", "1,3,2,3,1,2,2,3,1,2,3,2", new MinMax(0f, 0.5f), 0.3f), new DiveKick(700f, new MinMax(25f, 35f), new MinMax(350f, 430f)), new JumpRoll(100f, new MinMax(50f, 100f), new MinMax(200f, 300f), "B", 0.3f, new MinMax(0f, 0.3f)), new Shuriken(850f, 20f, 625f, 150f, 20f, 625f, 125f, 2), new Projectile(750f, 6f, 2f), new Umbrella(0.5f, 430f, 265f, 2, 1f, 2f, 745f, 1300f, 0.5f, 10f), new Kiss(170f, "P,P", 5.6f, 175f, 0.5f), new Teleport("0,100,-100,0,200,-200,50,-200,0,150", new MinMax(300f, 650f), 6.5f, 0.1f, 0.2f, 1f), new Baby(410f, 1f, 20, new MinMax(1.3f, 2.1f), new string[4] { "1,5,4,6,2,9,3,7,8,1,7,4,6,2,9,3,5,8", "3,9,7,1,4,2,5,8,6,3,2,7,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,6,9,3,8,7,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.1f), new Nun(410f, 0.7f, 20, new MinMax(1.3f, 2.1f), new string[4] { "1,5,4,2,9,3,8,1,7,4,6,2,3,5", "3,7,1,4,2,5,3,2,1,4,9,5,8,6", "5,6,9,3,1,7,4,2,8,5,3,4,2,1", "1,3,5,7,8,2,4,6,9,1,5,7,8,3,2,4,6,9" }, 0.3f), new Husband(10f, new MinMax(2f, 4f), 475f, 1f), new General(new string[2] { "L,L,M,L,T,L,M,T,L,L,M,T", "L,M,T,L,M,L,T,L,M,T,L,L,M,L,T" }, new MinMax(0.5f, 1.2f), 3.5f, 400f, 80f), new Lightning(650f, "140,190,165,150,130,180,145,165,140,175,155,160", new MinMax(4f, 6f), "3,2,4,2,3,4,2,3,4,3,3,4,4,3", new MinMax(0.5f, 0.9f), "1100,540,320,1170,655,827,344,916,626,1050,1200,360,710,1100,440,844,570,1150,377,687,954,515,905"), new Meteor(455f, 35, 100f, 200f, 1f, 2f, 1f, "1000,750,950,800,1100,850,750,900,1200,950,750,900,1150"), new Tidal(450f, 1.2f, 0.1f), new Roses(new MinMax(300f, 550f), 2f, 0f, new string[1] { "100,500,200,1100,800,600,400,1000,700,100,900,300,600,1000" }, new MinMax(2.5f, 3.5f), new MinMax(4f, 7f))));
				break;
			}
			return new SallyStagePlay(hp, goalTimes, list.ToArray());
		}
	}

	public class ShmupTutorial : AbstractLevelProperties<ShmupTutorial.State, ShmupTutorial.Pattern, ShmupTutorial.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected ShmupTutorial properties { get; private set; }

			public virtual void LevelInit(ShmupTutorial properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Default = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public State(float healthTrigger, Pattern[][] patterns, States stateName)
				: base(healthTrigger, patterns, stateName)
			{
			}
		}

		public ShmupTutorial(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 100f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "D")
			{
				return Pattern.Default;
			}
			Debug.LogError("Pattern ShmupTutorial.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static ShmupTutorial GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			case Level.Mode.Normal:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			}
			return new ShmupTutorial(hp, goalTimes, list.ToArray());
		}
	}

	public class Slime : AbstractLevelProperties<Slime.State, Slime.Pattern, Slime.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Slime properties { get; private set; }

			public virtual void LevelInit(Slime properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			BigSlime = 2,
			Tombstone = 3
		}

		public enum Pattern
		{
			Jump = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Jump jump;

			public readonly Punch punch;

			public readonly Tombstone tombstone;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Jump jump, Punch punch, Tombstone tombstone)
				: base(healthTrigger, patterns, stateName)
			{
				this.jump = jump;
				this.punch = punch;
				this.tombstone = tombstone;
			}
		}

		public class Jump : AbstractLevelPropertyGroup
		{
			public readonly float groundDelay;

			public readonly float highJumpVerticalSpeed;

			public readonly float highJumpHorizontalSpeed;

			public readonly float highJumpGravity;

			public readonly float lowJumpVerticalSpeed;

			public readonly float lowJumpHorizontalSpeed;

			public readonly float lowJumpGravity;

			public readonly MinMax numJumps;

			public readonly string patternString;

			public readonly int bigSlimeInitialJumpPunchCount;

			public Jump(float groundDelay, float highJumpVerticalSpeed, float highJumpHorizontalSpeed, float highJumpGravity, float lowJumpVerticalSpeed, float lowJumpHorizontalSpeed, float lowJumpGravity, MinMax numJumps, string patternString, int bigSlimeInitialJumpPunchCount)
			{
				this.groundDelay = groundDelay;
				this.highJumpVerticalSpeed = highJumpVerticalSpeed;
				this.highJumpHorizontalSpeed = highJumpHorizontalSpeed;
				this.highJumpGravity = highJumpGravity;
				this.lowJumpVerticalSpeed = lowJumpVerticalSpeed;
				this.lowJumpHorizontalSpeed = lowJumpHorizontalSpeed;
				this.lowJumpGravity = lowJumpGravity;
				this.numJumps = numJumps;
				this.patternString = patternString;
				this.bigSlimeInitialJumpPunchCount = bigSlimeInitialJumpPunchCount;
			}
		}

		public class Punch : AbstractLevelPropertyGroup
		{
			public readonly float preHold;

			public readonly float mainHold;

			public Punch(float preHold, float mainHold)
			{
				this.preHold = preHold;
				this.mainHold = mainHold;
			}
		}

		public class Tombstone : AbstractLevelPropertyGroup
		{
			public readonly float moveSpeed;

			public readonly MinMax attackDelay;

			public readonly float anticipationHold;

			public readonly string attackOffsetString;

			public readonly float tinyMeltDelay;

			public readonly float tinyRunTime;

			public readonly float tinyHealth;

			public readonly float tinyTimeUntilUnmelt;

			public Tombstone(float moveSpeed, MinMax attackDelay, float anticipationHold, string attackOffsetString, float tinyMeltDelay, float tinyRunTime, float tinyHealth, float tinyTimeUntilUnmelt)
			{
				this.moveSpeed = moveSpeed;
				this.attackDelay = attackDelay;
				this.anticipationHold = anticipationHold;
				this.attackOffsetString = attackOffsetString;
				this.tinyMeltDelay = tinyMeltDelay;
				this.tinyRunTime = tinyRunTime;
				this.tinyHealth = tinyHealth;
				this.tinyTimeUntilUnmelt = tinyTimeUntilUnmelt;
			}
		}

		public Slime(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1000f;
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.8f));
				timeline.events.Add(new Level.Timeline.Event("BigSlime", 0.56f));
				break;
			case Level.Mode.Normal:
				timeline.health = 1200f;
				timeline.events.Add(new Level.Timeline.Event("BigSlime", 0.76f));
				timeline.events.Add(new Level.Timeline.Event("Tombstone", 0.31f));
				break;
			case Level.Mode.Hard:
				timeline.health = 1400f;
				timeline.events.Add(new Level.Timeline.Event("BigSlime", 0.76f));
				timeline.events.Add(new Level.Timeline.Event("Tombstone", 0.36f));
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "J")
			{
				return Pattern.Jump;
			}
			Debug.LogError("Pattern Slime.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static Slime GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Jump(0.35f, 2300f, 700f, 6500f, 2200f, 670f, 6500f, new MinMax(5f, 8f), "LJ,LJ,HJ,LJ,RJ,HJ,LJ,HJ,LJ,HJ", 4), new Punch(0.85f, 0.4f), new Tombstone(0f, new MinMax(0f, 1f), 0f, string.Empty, 0f, 0f, 0f, 0f)));
				list.Add(new State(0.8f, new Pattern[1][] { new Pattern[1] }, States.Generic, new Jump(0.35f, 2350f, 750f, 6500f, 2200f, 700f, 6500f, new MinMax(5f, 8f), "HJ,HJ,LJ,HJ,LJ,RJ,HJ,LJ,LJ,HJ,LJ", 6), new Punch(0.85f, 0.4f), new Tombstone(0f, new MinMax(0f, 1f), 0f, string.Empty, 0f, 0f, 0f, 0f)));
				list.Add(new State(0.56f, new Pattern[1][] { new Pattern[1] }, States.BigSlime, new Jump(0.45f, 2850f, 820f, 7500f, 2320f, 785f, 7500f, new MinMax(5f, 8f), "LJ,HJ,RJ,LJ,HJ,RJ,LJ,LJ,HJ,HJ,LJ,HJ,LJ,HJ", 2), new Punch(0.65f, 0.4f), new Tombstone(0f, new MinMax(0f, 1f), 0f, string.Empty, 0f, 0f, 0f, 0f)));
				break;
			case Level.Mode.Normal:
				hp = 1200;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Jump(0.2f, 2350f, 840f, 6500f, 2000f, 780f, 6500f, new MinMax(5f, 8f), "HJ,LJ,HJ,LJ,D.5, LJ,RJ,HJ,LJ,HJ,LJ,LJ,HJ,RJ,LJ,LJ", 3), new Punch(0.65f, 0.4f), new Tombstone(950f, new MinMax(2f, 4f), 0.35f, "-130,70,0,130,-70,0", 0.2f, 3.1f, 3.9f, 3.4f)));
				list.Add(new State(0.76f, new Pattern[1][] { new Pattern[0] }, States.BigSlime, new Jump(0.3f, 2850f, 820f, 7500f, 2250f, 760f, 7500f, new MinMax(5f, 8f), "HJ,HJ,LJ,HJ,RJ,LJ,D.5, LJ,HL,LJ,RJ,HJ,LJ,LJ,HJ,LJ,LJ", 2), new Punch(0.65f, 0.4f), new Tombstone(950f, new MinMax(2f, 4f), 0.35f, "-130,70,0,130,-70,0", 0.2f, 3.1f, 3.9f, 3.4f)));
				list.Add(new State(0.31f, new Pattern[1][] { new Pattern[0] }, States.Tombstone, new Jump(0.3f, 2850f, 820f, 7500f, 2250f, 760f, 7500f, new MinMax(5f, 8f), "HJ,HJ,LJ,HJ,RJ,LJ,D.5, LJ,HL,LJ,RJ,HJ,LJ,LJ,HJ,LJ,LJ", 2), new Punch(0.65f, 0.4f), new Tombstone(950f, new MinMax(2f, 4f), 0.35f, "-130,70,0,130,-70,0", 0.2f, 3.1f, 3.9f, 3.4f)));
				break;
			case Level.Mode.Hard:
				hp = 1400;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Jump(0.001f, 2585f, 840f, 7150f, 2200f, 780f, 7150f, new MinMax(5f, 8f), "HJ,LJ,HJ,LJ,D.5, LJ,RJ,HJ,LJ,HJ,LJ,LJ,HJ,RJ,LJ,LJ", 3), new Punch(0.65f, 0.4f), new Tombstone(1050f, new MinMax(1.5f, 4f), 0.35f, "-150,80,0,140,-80,0,-160,0,150,-90,0", 0.2f, 3.1f, 3.9f, 3f)));
				list.Add(new State(0.76f, new Pattern[1][] { new Pattern[0] }, States.BigSlime, new Jump(0.05f, 2565f, 738f, 7200f, 2025f, 685f, 7200f, new MinMax(5f, 8f), "HJ,LJ,HJ,RJ,LJ,LJ,D.5, LJ,RJ,HL,LJ,RJ,HJ,LJ,HJ,LJ,LJ", 2), new Punch(0.65f, 0.4f), new Tombstone(1050f, new MinMax(1.5f, 4f), 0.35f, "-150,80,0,140,-80,0,-160,0,150,-90,0", 0.2f, 3.1f, 3.9f, 3f)));
				list.Add(new State(0.36f, new Pattern[1][] { new Pattern[0] }, States.Tombstone, new Jump(0.05f, 2565f, 738f, 7200f, 2025f, 685f, 7200f, new MinMax(5f, 8f), "HJ,LJ,HJ,RJ,LJ,LJ,D.5, LJ,RJ,HL,LJ,RJ,HJ,LJ,HJ,LJ,LJ", 2), new Punch(0.65f, 0.4f), new Tombstone(1050f, new MinMax(1.5f, 4f), 0.35f, "-150,80,0,140,-80,0,-160,0,150,-90,0", 0.2f, 3.1f, 3.9f, 3f)));
				break;
			}
			return new Slime(hp, goalTimes, list.ToArray());
		}
	}

	public class Test : AbstractLevelProperties<Test.State, Test.Pattern, Test.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Test properties { get; private set; }

			public virtual void LevelInit(Test properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1,
			Test = 2,
			SecondTest = 3
		}

		public enum Pattern
		{
			Main = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Moving moving;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Moving moving)
				: base(healthTrigger, patterns, stateName)
			{
				this.moving = moving;
			}
		}

		public class Moving : AbstractLevelPropertyGroup
		{
			public readonly MinMax timeX;

			public readonly MinMax timeY;

			public readonly MinMax timeScale;

			public Moving(MinMax timeX, MinMax timeY, MinMax timeScale)
			{
				this.timeX = timeX;
				this.timeY = timeY;
				this.timeScale = timeScale;
			}
		}

		public Test(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 1001f;
				break;
			case Level.Mode.Normal:
				timeline.health = 100f;
				timeline.events.Add(new Level.Timeline.Event("Test", 0.5f));
				timeline.events.Add(new Level.Timeline.Event("SecondTest", 0.1f));
				timeline.events.Add(new Level.Timeline.Event("Generic", 0.05f));
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "M")
			{
				return Pattern.Main;
			}
			Debug.LogError("Pattern Test.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static Test GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 1001;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Moving(new MinMax(20f, 5f), new MinMax(20f, 5f), new MinMax(20f, 5f))));
				break;
			case Level.Mode.Normal:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[3] }, States.Main, new Moving(new MinMax(7f, 2f), new MinMax(3f, 0.2f), new MinMax(1f, 0.3f))));
				list.Add(new State(0.5f, new Pattern[1][] { new Pattern[1] }, States.Test, new Moving(new MinMax(7f, 2f), new MinMax(3f, 0.2f), new MinMax(1f, 0.3f))));
				list.Add(new State(0.1f, new Pattern[1][] { new Pattern[1] }, States.SecondTest, new Moving(new MinMax(7f, 2f), new MinMax(3f, 0.2f), new MinMax(1f, 0.3f))));
				list.Add(new State(0.05f, new Pattern[1][] { new Pattern[1] }, States.Generic, new Moving(new MinMax(7f, 2f), new MinMax(3f, 0.2f), new MinMax(1f, 0.3f))));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new Moving(new MinMax(1f, 0.1f), new MinMax(1f, 0.1f), new MinMax(1f, 0.1f))));
				break;
			}
			return new Test(hp, goalTimes, list.ToArray());
		}
	}

	public class Train : AbstractLevelProperties<Train.State, Train.Pattern, Train.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Train properties { get; private set; }

			public virtual void LevelInit(Train properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Train = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly BlindSpecter blindSpecter;

			public readonly Skeleton skeleton;

			public readonly LollipopGhouls lollipopGhouls;

			public readonly Engine engine;

			public readonly Pumpkins pumpkins;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, BlindSpecter blindSpecter, Skeleton skeleton, LollipopGhouls lollipopGhouls, Engine engine, Pumpkins pumpkins)
				: base(healthTrigger, patterns, stateName)
			{
				this.blindSpecter = blindSpecter;
				this.skeleton = skeleton;
				this.lollipopGhouls = lollipopGhouls;
				this.engine = engine;
				this.pumpkins = pumpkins;
			}
		}

		public class BlindSpecter : AbstractLevelPropertyGroup
		{
			public readonly int health;

			public readonly int attackLoops;

			public readonly MinMax heightMax;

			public readonly MinMax timeX;

			public readonly MinMax timeY;

			public readonly float hesitate;

			public readonly float eyeHealth;

			public BlindSpecter(int health, int attackLoops, MinMax heightMax, MinMax timeX, MinMax timeY, float hesitate, float eyeHealth)
			{
				this.health = health;
				this.attackLoops = attackLoops;
				this.heightMax = heightMax;
				this.timeX = timeX;
				this.timeY = timeY;
				this.hesitate = hesitate;
				this.eyeHealth = eyeHealth;
			}
		}

		public class Skeleton : AbstractLevelPropertyGroup
		{
			public readonly float health;

			public readonly MinMax attackDelay;

			public readonly float appearDelay;

			public readonly float slapHoldTime;

			public Skeleton(float health, MinMax attackDelay, float appearDelay, float slapHoldTime)
			{
				this.health = health;
				this.attackDelay = attackDelay;
				this.appearDelay = appearDelay;
				this.slapHoldTime = slapHoldTime;
			}
		}

		public class LollipopGhouls : AbstractLevelPropertyGroup
		{
			public readonly float health;

			public readonly float initDelay;

			public readonly float mainDelay;

			public readonly float warningTime;

			public readonly float moveTime;

			public readonly float moveDistance;

			public readonly float cannonDelay;

			public readonly float ghostDelay;

			public readonly float ghostSpeed;

			public readonly float ghostAimSpeed;

			public readonly float ghostHealth;

			public readonly float skullSpeed;

			public LollipopGhouls(float health, float initDelay, float mainDelay, float warningTime, float moveTime, float moveDistance, float cannonDelay, float ghostDelay, float ghostSpeed, float ghostAimSpeed, float ghostHealth, float skullSpeed)
			{
				this.health = health;
				this.initDelay = initDelay;
				this.mainDelay = mainDelay;
				this.warningTime = warningTime;
				this.moveTime = moveTime;
				this.moveDistance = moveDistance;
				this.cannonDelay = cannonDelay;
				this.ghostDelay = ghostDelay;
				this.ghostSpeed = ghostSpeed;
				this.ghostAimSpeed = ghostAimSpeed;
				this.ghostHealth = ghostHealth;
				this.skullSpeed = skullSpeed;
			}
		}

		public class Engine : AbstractLevelPropertyGroup
		{
			public readonly float health;

			public readonly float forwardTime;

			public readonly float backTime;

			public readonly MinMax doorTime;

			public readonly float tailDelay;

			public readonly float maxDist;

			public readonly float minDist;

			public readonly float fireDelay;

			public readonly int fireGravity;

			public readonly MinMax fireVelocityX;

			public readonly MinMax fireVelocityY;

			public readonly float projectileDelay;

			public readonly float projectileUpSpeed;

			public readonly float projectileXSpeed;

			public readonly float projectileGravity;

			public Engine(float health, float forwardTime, float backTime, MinMax doorTime, float tailDelay, float maxDist, float minDist, float fireDelay, int fireGravity, MinMax fireVelocityX, MinMax fireVelocityY, float projectileDelay, float projectileUpSpeed, float projectileXSpeed, float projectileGravity)
			{
				this.health = health;
				this.forwardTime = forwardTime;
				this.backTime = backTime;
				this.doorTime = doorTime;
				this.tailDelay = tailDelay;
				this.maxDist = maxDist;
				this.minDist = minDist;
				this.fireDelay = fireDelay;
				this.fireGravity = fireGravity;
				this.fireVelocityX = fireVelocityX;
				this.fireVelocityY = fireVelocityY;
				this.projectileDelay = projectileDelay;
				this.projectileUpSpeed = projectileUpSpeed;
				this.projectileXSpeed = projectileXSpeed;
				this.projectileGravity = projectileGravity;
			}
		}

		public class Pumpkins : AbstractLevelPropertyGroup
		{
			public readonly string bossPhaseOn;

			public readonly float health;

			public readonly float speed;

			public readonly float fallTime;

			public readonly float delay;

			public Pumpkins(string bossPhaseOn, float health, float speed, float fallTime, float delay)
			{
				this.bossPhaseOn = bossPhaseOn;
				this.health = health;
				this.speed = speed;
				this.fallTime = fallTime;
				this.delay = delay;
			}
		}

		public Train(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 500f;
				break;
			case Level.Mode.Normal:
				timeline.health = 500f;
				break;
			case Level.Mode.Hard:
				timeline.health = 500f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "T")
			{
				return Pattern.Train;
			}
			Debug.LogError("Pattern Train.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static Train GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 500;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new BlindSpecter(400, 2, new MinMax(150f, 300f), new MinMax(3.5f, 4.5f), new MinMax(0.7f, 0.9f), 3f, 3.5f), new Skeleton(320f, new MinMax(3.5f, 1.5f), 2f, 2.3f), new LollipopGhouls(180f, 2f, 1.5f, 1f, 3.3f, 600f, 5.5f, 1f, 250f, 1.35f, 5f, 800f), new Engine(230f, 3.3f, 5f, new MinMax(4f, 6f), 1.8f, 0f, 0f, 0.25f, 1000, new MinMax(-300f, 300f), new MinMax(500f, 800f), 4.2f, 650f, 1000f, 1000f), new Pumpkins("0", 4f, 200f, 1.5f, 5f)));
				break;
			case Level.Mode.Normal:
				hp = 500;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new BlindSpecter(425, 2, new MinMax(60f, 220f), new MinMax(3.9f, 4.8f), new MinMax(0.5f, 0.7f), 1.4f, 3.5f), new Skeleton(325f, new MinMax(2.5f, 0.5f), 2f, 2f), new LollipopGhouls(200f, 2f, 1.25f, 1f, 2.6f, 700f, 3.4f, 1f, 250f, 1.35f, 5f, 900f), new Engine(200f, 3.3f, 5f, new MinMax(4.5f, 6f), 1f, 300f, -425f, 0.45f, 800, new MinMax(-350f, 350f), new MinMax(350f, 600f), 4.2f, 650f, 750f, 1000f), new Pumpkins("1,2", 4f, 200f, 1.5f, 5f)));
				break;
			case Level.Mode.Hard:
				hp = 500;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main, new BlindSpecter(425, 3, new MinMax(100f, 300f), new MinMax(3.4f, 4.3f), new MinMax(0.5f, 0.7f), 1.4f, 3.5f), new Skeleton(325f, new MinMax(2f, 0.5f), 2f, 1.5f), new LollipopGhouls(200f, 1.5f, 0.5f, 1f, 1.8f, 600f, 2.7f, 1f, 275f, 1.35f, 5f, 900f), new Engine(200f, 2.9f, 3.6f, new MinMax(4.5f, 6f), 1f, -300f, 425f, 0.35f, 800, new MinMax(-325f, 325f), new MinMax(400f, 650f), 3.8f, 650f, 850f, 1000f), new Pumpkins("1,2,4", 4f, 250f, 1.5f, 4.5f)));
				break;
			}
			return new Train(hp, goalTimes, list.ToArray());
		}
	}

	public class Tutorial : AbstractLevelProperties<Tutorial.State, Tutorial.Pattern, Tutorial.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Tutorial properties { get; private set; }

			public virtual void LevelInit(Tutorial properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			A = 0,
			Uninitialized = 1
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public State(float healthTrigger, Pattern[][] patterns, States stateName)
				: base(healthTrigger, patterns, stateName)
			{
			}
		}

		public Tutorial(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 5000f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			if (id != null && id == "A")
			{
				return Pattern.A;
			}
			Debug.LogError("Pattern Tutorial.Pattern for  " + id + " not found.");
			return Pattern.Uninitialized;
		}

		public static Tutorial GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main));
				break;
			case Level.Mode.Normal:
				hp = 5000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[1] }, States.Main));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main));
				break;
			}
			return new Tutorial(hp, goalTimes, list.ToArray());
		}
	}

	public class Veggies : AbstractLevelProperties<Veggies.State, Veggies.Pattern, Veggies.States>
	{
		public class Entity : AbstractLevelEntity
		{
			protected Veggies properties { get; private set; }

			public virtual void LevelInit(Veggies properties)
			{
				this.properties = properties;
			}

			public virtual void LevelInitWithGroup(AbstractLevelPropertyGroup propertyGroup)
			{
			}
		}

		public enum States
		{
			Main = 0,
			Generic = 1
		}

		public enum Pattern
		{
			Potato = 0,
			Onion = 1,
			Beet = 2,
			Peas = 3,
			Carrot = 4,
			Uninitialized = 5
		}

		public class State : AbstractLevelState<Pattern, States>
		{
			public readonly Potato potato;

			public readonly Onion onion;

			public readonly Beet beet;

			public readonly Peas peas;

			public readonly Carrot carrot;

			public State(float healthTrigger, Pattern[][] patterns, States stateName, Potato potato, Onion onion, Beet beet, Peas peas, Carrot carrot)
				: base(healthTrigger, patterns, stateName)
			{
				this.potato = potato;
				this.onion = onion;
				this.beet = beet;
				this.peas = peas;
				this.carrot = carrot;
			}
		}

		public class Potato : AbstractLevelPropertyGroup
		{
			public readonly int hp;

			public readonly float idleTime;

			public readonly int seriesCount;

			public readonly float seriesDelay;

			public readonly int bulletCount;

			public readonly MinMax bulletDelay;

			public readonly float bulletSpeed;

			public Potato(int hp, float idleTime, int seriesCount, float seriesDelay, int bulletCount, MinMax bulletDelay, float bulletSpeed)
			{
				this.hp = hp;
				this.idleTime = idleTime;
				this.seriesCount = seriesCount;
				this.seriesDelay = seriesDelay;
				this.bulletCount = bulletCount;
				this.bulletDelay = bulletDelay;
				this.bulletSpeed = bulletSpeed;
			}
		}

		public class Onion : AbstractLevelPropertyGroup
		{
			public readonly int hp;

			public readonly float happyTime;

			public readonly MinMax cryLoops;

			public readonly string[] tearPatterns;

			public readonly float tearAnticipate;

			public readonly float tearCommaDelay;

			public readonly float tearTime;

			public readonly MinMax pinkTearRange;

			public readonly float heartMaxSpeed;

			public readonly float heartAcceleration;

			public readonly float heartBounceRatio;

			public readonly int heartHP;

			public Onion(int hp, float happyTime, MinMax cryLoops, string[] tearPatterns, float tearAnticipate, float tearCommaDelay, float tearTime, MinMax pinkTearRange, float heartMaxSpeed, float heartAcceleration, float heartBounceRatio, int heartHP)
			{
				this.hp = hp;
				this.happyTime = happyTime;
				this.cryLoops = cryLoops;
				this.tearPatterns = tearPatterns;
				this.tearAnticipate = tearAnticipate;
				this.tearCommaDelay = tearCommaDelay;
				this.tearTime = tearTime;
				this.pinkTearRange = pinkTearRange;
				this.heartMaxSpeed = heartMaxSpeed;
				this.heartAcceleration = heartAcceleration;
				this.heartBounceRatio = heartBounceRatio;
				this.heartHP = heartHP;
			}
		}

		public class Beet : AbstractLevelPropertyGroup
		{
			public readonly int hp;

			public readonly float idleTime;

			public readonly string[] babyPatterns;

			public readonly float babySpeedUp;

			public readonly int babySpeedSpread;

			public readonly float babySpreadAngle;

			public readonly float babyDelay;

			public readonly float babyGroupDelay;

			public readonly MinMax alternateRate;

			public Beet(int hp, float idleTime, string[] babyPatterns, float babySpeedUp, int babySpeedSpread, float babySpreadAngle, float babyDelay, float babyGroupDelay, MinMax alternateRate)
			{
				this.hp = hp;
				this.idleTime = idleTime;
				this.babyPatterns = babyPatterns;
				this.babySpeedUp = babySpeedUp;
				this.babySpeedSpread = babySpeedSpread;
				this.babySpreadAngle = babySpreadAngle;
				this.babyDelay = babyDelay;
				this.babyGroupDelay = babyGroupDelay;
				this.alternateRate = alternateRate;
			}
		}

		public class Peas : AbstractLevelPropertyGroup
		{
			public readonly int hp;

			public Peas(int hp)
			{
				this.hp = hp;
			}
		}

		public class Carrot : AbstractLevelPropertyGroup
		{
			public readonly int hp;

			public readonly float startIdleTime;

			public readonly MinMax idleRange;

			public readonly int bulletCount;

			public readonly float bulletDelay;

			public readonly float bulletSpeed;

			public readonly float homingInitDelay;

			public readonly float homingSpeed;

			public readonly float homingRotation;

			public readonly int homingHP;

			public readonly float homingDelay;

			public readonly MinMax homingDuration;

			public readonly float homingBgSpeed;

			public readonly MinMax homingNumOfCarrots;

			public Carrot(int hp, float startIdleTime, MinMax idleRange, int bulletCount, float bulletDelay, float bulletSpeed, float homingInitDelay, float homingSpeed, float homingRotation, int homingHP, float homingDelay, MinMax homingDuration, float homingBgSpeed, MinMax homingNumOfCarrots)
			{
				this.hp = hp;
				this.startIdleTime = startIdleTime;
				this.idleRange = idleRange;
				this.bulletCount = bulletCount;
				this.bulletDelay = bulletDelay;
				this.bulletSpeed = bulletSpeed;
				this.homingInitDelay = homingInitDelay;
				this.homingSpeed = homingSpeed;
				this.homingRotation = homingRotation;
				this.homingHP = homingHP;
				this.homingDelay = homingDelay;
				this.homingDuration = homingDuration;
				this.homingBgSpeed = homingBgSpeed;
				this.homingNumOfCarrots = homingNumOfCarrots;
			}
		}

		public Veggies(int hp, Level.GoalTimes goalTimes, State[] states)
			: base((float)hp, goalTimes, states)
		{
		}

		public Level.Timeline CreateTimeline(Level.Mode mode)
		{
			Level.Timeline timeline = new Level.Timeline();
			switch (mode)
			{
			default:
				timeline.health = 100f;
				break;
			case Level.Mode.Normal:
				timeline.health = 1000f;
				break;
			case Level.Mode.Hard:
				timeline.health = 100f;
				break;
			}
			return timeline;
		}

		public static Pattern GetPatternByID(string id)
		{
			id = id.ToUpper();
			switch (id)
			{
			case "P":
				return Pattern.Potato;
			case "O":
				return Pattern.Onion;
			case "B":
				return Pattern.Beet;
			case "H":
				return Pattern.Peas;
			case "C":
				return Pattern.Carrot;
			default:
				Debug.LogError("Pattern Veggies.Pattern for  " + id + " not found.");
				return Pattern.Uninitialized;
			}
		}

		public static Veggies GetMode(Level.Mode mode)
		{
			int hp = 0;
			Level.GoalTimes goalTimes = null;
			List<State> list = new List<State>();
			switch (mode)
			{
			case Level.Mode.Easy:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main, new Potato(450, 3f, 3, 1.2f, 3, new MinMax(0.4f, 0.7f), -600f), new Onion(0, 0f, new MinMax(0f, 1f), new string[0], 0f, 0f, 0f, new MinMax(0f, 1f), 0f, 0f, 0f, 0), new Beet(0, 0f, new string[0], 0f, 0, 0f, 0f, 0f, new MinMax(0f, 1f)), new Peas(0), new Carrot(450, 0f, new MinMax(6f, 7f), 2, 2f, 700f, 1f, 200f, 2.2f, 4, 1.5f, new MinMax(0f, 0f), 550f, new MinMax(3f, 5f))));
				break;
			case Level.Mode.Normal:
				hp = 1000;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[3]
				{
					Pattern.Potato,
					Pattern.Onion,
					Pattern.Carrot
				} }, States.Main, new Potato(360, 3f, 3, 1f, 4, new MinMax(0.2f, 0.6f), -700f), new Onion(400, 5f, new MinMax(38f, 38f), new string[7] { "280,630,400,500,335,450,280,635,535,280,525", "450,280,350,630,280,560,460,335,630,510,360", "630,280,470,550,320,400,280,630,505,360,280", "450,550,350,630,280,400,300,500,280,630,315", "330,630,415,280,520,630,280,450,500,280,570", "450,630,400,280,330,570,450,350,280,630,500", "630,400,280,550,630,350,280,450,500,280,400" }, 0.2f, 0.2f, 1.1f, new MinMax(4f, 6f), 750f, 1000f, 2.5f, 250), new Beet(5, 2f, new string[1] { "2,4,2" }, 800f, 800, 45f, 1f, 2f, new MinMax(4f, 7f)), new Peas(5), new Carrot(475, 0f, new MinMax(6.8f, 8f), 3, 1.5f, 800f, 1f, 250f, 2.3f, 4, 1.2f, new MinMax(0f, 0f), 550f, new MinMax(4f, 6f))));
				break;
			case Level.Mode.Hard:
				hp = 100;
				goalTimes = new Level.GoalTimes(120f, 120f, 120f);
				list.Add(new State(10f, new Pattern[1][] { new Pattern[0] }, States.Main, new Potato(400, 2.5f, 3, 1f, 4, new MinMax(0.1f, 0.4f), -1000f), new Onion(425, 5f, new MinMax(38f, 38f), new string[7] { "280,500,330,630,280,400,350,550,280,630,300", "630,280,400,500,320,450,280,630,505,330,630", "280,630,350,550,405,460,280,635,535,280,600", "330,480,630,280,520,630,280,350,450,280,570", "450,630,350,280,400,560,460,325,630,280,420", "630,450,280,330,550,415,630,580,280,500,330", "330,280,505,630,450,630,280,580,330,400,550" }, 0.1f, 0.005f, 0.9f, new MinMax(5f, 7f), 800f, 1000f, 2f, 250), new Beet(0, 0f, new string[0], 0f, 0, 0f, 0f, 0f, new MinMax(0f, 1f)), new Peas(0), new Carrot(475, 1.5f, new MinMax(5f, 6f), 2, 1.5f, 850f, 1f, 260f, 2.3f, 4, 0.8f, new MinMax(0f, 0f), 550f, new MinMax(5f, 7f))));
				break;
			}
			return new Veggies(hp, goalTimes, list.ToArray());
		}
	}

	public static string[] levels = new string[51]
	{
		"scene_level_test", "scene_level_flying_test", "scene_level_tutorial", "scene_level_pirate", "scene_level_bat", "scene_level_train", "scene_level_veggies", "scene_level_frogs", "scene_level_bee", "scene_level_mouse",
		"scene_level_dragon", "scene_level_flower", "scene_level_slime", "scene_level_baroness", "scene_level_airship_jelly", "scene_level_airship_stork", "scene_level_airship_crab", "scene_level_flying_bird", "scene_level_flying_mermaid", "scene_level_flying_blimp",
		"scene_level_robot", "scene_level_clown", "scene_level_sally_stage_play", "scene_level_dice_palace_domino", "scene_level_dice_palace_card", "scene_level_dice_palace_chips", "scene_level_dice_palace_cigar", "scene_level_dice_palace_test", "scene_level_dice_palace_booze", "scene_level_dice_palace_roulette",
		"scene_level_dice_palace_pachinko", "scene_level_dice_palace_rabbit", "scene_level_airship_clam", "scene_level_flying_genie", "scene_level_dice_palace_light", "scene_level_dice_palace_flying_horse", "scene_level_dice_palace_flying_memory", "scene_level_dice_palace_main", "scene_level_dice_palace_eight_ball", "scene_level_devil",
		"scene_level_retro_arcade", "scene_level_mausoleum", "scene_level_house_elder_kettle", "scene_level_dice_gate", "scene_level_shmup_tutorial", "scene_level_platforming_1_1F", "scene_level_platforming_1_2F", "scene_level_platforming_3_1F", "scene_level_platforming_3_2F", "scene_level_platforming_2_2F",
		"scene_level_platforming_2_1F"
	};

	public static string GetLevelScene(Levels level)
	{
		switch (level)
		{
		default:
			return string.Empty;
		case Levels.Test:
			return "scene_level_test";
		case Levels.FlyingTest:
			return "scene_level_flying_test";
		case Levels.Tutorial:
			return "scene_level_tutorial";
		case Levels.Pirate:
			return "scene_level_pirate";
		case Levels.Bat:
			return "scene_level_bat";
		case Levels.Train:
			return "scene_level_train";
		case Levels.Veggies:
			return "scene_level_veggies";
		case Levels.Frogs:
			return "scene_level_frogs";
		case Levels.Bee:
			return "scene_level_bee";
		case Levels.Mouse:
			return "scene_level_mouse";
		case Levels.Dragon:
			return "scene_level_dragon";
		case Levels.Flower:
			return "scene_level_flower";
		case Levels.Slime:
			return "scene_level_slime";
		case Levels.Baroness:
			return "scene_level_baroness";
		case Levels.AirshipJelly:
			return "scene_level_airship_jelly";
		case Levels.AirshipStork:
			return "scene_level_airship_stork";
		case Levels.AirshipCrab:
			return "scene_level_airship_crab";
		case Levels.FlyingBird:
			return "scene_level_flying_bird";
		case Levels.FlyingMermaid:
			return "scene_level_flying_mermaid";
		case Levels.FlyingBlimp:
			return "scene_level_flying_blimp";
		case Levels.Robot:
			return "scene_level_robot";
		case Levels.Clown:
			return "scene_level_clown";
		case Levels.SallyStagePlay:
			return "scene_level_sally_stage_play";
		case Levels.DicePalaceDomino:
			return "scene_level_dice_palace_domino";
		case Levels.DicePalaceCard:
			return "scene_level_dice_palace_card";
		case Levels.DicePalaceChips:
			return "scene_level_dice_palace_chips";
		case Levels.DicePalaceCigar:
			return "scene_level_dice_palace_cigar";
		case Levels.DicePalaceTest:
			return "scene_level_dice_palace_test";
		case Levels.DicePalaceBooze:
			return "scene_level_dice_palace_booze";
		case Levels.DicePalaceRoulette:
			return "scene_level_dice_palace_roulette";
		case Levels.DicePalacePachinko:
			return "scene_level_dice_palace_pachinko";
		case Levels.DicePalaceRabbit:
			return "scene_level_dice_palace_rabbit";
		case Levels.AirshipClam:
			return "scene_level_airship_clam";
		case Levels.FlyingGenie:
			return "scene_level_flying_genie";
		case Levels.DicePalaceLight:
			return "scene_level_dice_palace_light";
		case Levels.DicePalaceFlyingHorse:
			return "scene_level_dice_palace_flying_horse";
		case Levels.DicePalaceFlyingMemory:
			return "scene_level_dice_palace_flying_memory";
		case Levels.DicePalaceMain:
			return "scene_level_dice_palace_main";
		case Levels.DicePalaceEightBall:
			return "scene_level_dice_palace_eight_ball";
		case Levels.Devil:
			return "scene_level_devil";
		case Levels.RetroArcade:
			return "scene_level_retro_arcade";
		case Levels.Mausoleum:
			return "scene_level_mausoleum";
		case Levels.House:
			return "scene_level_house_elder_kettle";
		case Levels.DiceGate:
			return "scene_level_dice_gate";
		case Levels.ShmupTutorial:
			return "scene_level_shmup_tutorial";
		case Levels.Platforming_Level_1_1:
			return "scene_level_platforming_1_1F";
		case Levels.Platforming_Level_1_2:
			return "scene_level_platforming_1_2F";
		case Levels.Platforming_Level_3_1:
			return "scene_level_platforming_3_1F";
		case Levels.Platforming_Level_3_2:
			return "scene_level_platforming_3_2F";
		case Levels.Platforming_Level_2_2:
			return "scene_level_platforming_2_2F";
		case Levels.Platforming_Level_2_1:
			return "scene_level_platforming_2_1F";
		}
	}

	public static string[] GetLevelPatternNames(Levels level)
	{
		switch (level)
		{
		default:
			return new string[0];
		case Levels.Test:
			return new string[1] { "Main" };
		case Levels.FlyingTest:
			return new string[1] { "Main" };
		case Levels.Tutorial:
			return new string[1] { "A" };
		case Levels.Pirate:
			return new string[5] { "Shark", "Squid", "DogFish", "Peashot", "Boat" };
		case Levels.Bat:
			return new string[2] { "Bouncer", "Lightning" };
		case Levels.Train:
			return new string[1] { "Train" };
		case Levels.Veggies:
			return new string[5] { "Potato", "Onion", "Beet", "Peas", "Carrot" };
		case Levels.Frogs:
			return new string[6] { "TallFan", "ShortRage", "TallFireflies", "ShortClap", "Morph", "RagePlusFireflies" };
		case Levels.Bee:
			return new string[7] { "BlackHole", "Chain", "Triangle", "Follower", "SecurityGuard", "Wing", "Turbine" };
		case Levels.Mouse:
			return new string[10] { "Move", "Dash", "CherryBomb", "Catapult", "RomanCandle", "SawBlades", "Flame", "LeftClaw", "RightClaw", "GhostMouse" };
		case Levels.Dragon:
			return new string[2] { "Meteor", "Peashot" };
		case Levels.Flower:
			return new string[5] { "Laser", "PodHands", "GattlingGun", "VineHands", "Nothing" };
		case Levels.Slime:
			return new string[1] { "Jump" };
		case Levels.Baroness:
			return new string[1] { "Default" };
		case Levels.AirshipJelly:
			return new string[1] { "Main" };
		case Levels.AirshipStork:
			return new string[1] { "Default" };
		case Levels.AirshipCrab:
			return new string[1] { "Default" };
		case Levels.FlyingBird:
			return new string[7] { "Feathers", "Eggs", "Lasers", "SmallBird", "Garbage", "Heart", "Default" };
		case Levels.FlyingMermaid:
			return new string[8] { "Yell", "Summon", "Fish", "Zap", "Eel", "Bubble", "HeadBlast", "BubbleHeadBlast" };
		case Levels.FlyingBlimp:
			return new string[3] { "Dash", "Tornado", "Shoot" };
		case Levels.Robot:
			return new string[1] { "Default" };
		case Levels.Clown:
			return new string[1] { "Default" };
		case Levels.SallyStagePlay:
			return new string[4] { "Jump", "Umbrella", "Kiss", "Teleport" };
		case Levels.DicePalaceDomino:
			return new string[2] { "Boomerang", "BouncyBall" };
		case Levels.DicePalaceCard:
			return new string[1] { "Default" };
		case Levels.DicePalaceChips:
			return new string[1] { "Default" };
		case Levels.DicePalaceCigar:
			return new string[1] { "Default" };
		case Levels.DicePalaceTest:
			return new string[1] { "Default" };
		case Levels.DicePalaceBooze:
			return new string[1] { "Default" };
		case Levels.DicePalaceRoulette:
			return new string[3] { "Default", "Twirl", "Marble" };
		case Levels.DicePalacePachinko:
			return new string[1] { "Default" };
		case Levels.DicePalaceRabbit:
			return new string[2] { "MagicWand", "MagicParry" };
		case Levels.AirshipClam:
			return new string[2] { "Spit", "Barnacles" };
		case Levels.FlyingGenie:
			return new string[1] { "Default" };
		case Levels.DicePalaceLight:
			return new string[1] { "Default" };
		case Levels.DicePalaceFlyingHorse:
			return new string[1] { "Default" };
		case Levels.DicePalaceFlyingMemory:
			return new string[1] { "Default" };
		case Levels.DicePalaceMain:
			return new string[1] { "Default" };
		case Levels.DicePalaceEightBall:
			return new string[1] { "Default" };
		case Levels.Devil:
			return new string[8] { "Default", "SplitDevilProjectileAttack", "SplitDevilWallAttack", "Clap", "Head", "Pitchfork", "BombEye", "SkullEye" };
		case Levels.RetroArcade:
			return new string[1] { "Default" };
		case Levels.Mausoleum:
			return new string[1] { "Default" };
		case Levels.House:
			return new string[1] { "Default" };
		case Levels.DiceGate:
			return new string[1] { "Default" };
		case Levels.ShmupTutorial:
			return new string[1] { "Default" };
		}
	}

	public static Levels GetPlatformingLevelLevel(PlatformingLevels platformingLevel)
	{
		switch (platformingLevel)
		{
		default:
			return Levels.Platforming_Level_1_1;
		case PlatformingLevels.Platforming_Level_1_1:
			return Levels.Platforming_Level_1_1;
		case PlatformingLevels.Platforming_Level_1_2:
			return Levels.Platforming_Level_1_2;
		case PlatformingLevels.Platforming_Level_3_1:
			return Levels.Platforming_Level_3_1;
		case PlatformingLevels.Platforming_Level_3_2:
			return Levels.Platforming_Level_3_2;
		case PlatformingLevels.Platforming_Level_2_2:
			return Levels.Platforming_Level_2_2;
		case PlatformingLevels.Platforming_Level_2_1:
			return Levels.Platforming_Level_2_1;
		}
	}

	public static Levels GetDicePalaceLevel(DicePalaceLevels level)
	{
		switch (level)
		{
		default:
			return Levels.DicePalaceDomino;
		case DicePalaceLevels.DicePalaceCard:
			return Levels.DicePalaceCard;
		case DicePalaceLevels.DicePalaceChips:
			return Levels.DicePalaceChips;
		case DicePalaceLevels.DicePalaceCigar:
			return Levels.DicePalaceCigar;
		case DicePalaceLevels.DicePalaceTest:
			return Levels.DicePalaceTest;
		case DicePalaceLevels.DicePalaceBooze:
			return Levels.DicePalaceBooze;
		case DicePalaceLevels.DicePalaceRoulette:
			return Levels.DicePalaceRoulette;
		case DicePalaceLevels.DicePalacePachinko:
			return Levels.DicePalacePachinko;
		case DicePalaceLevels.DicePalaceRabbit:
			return Levels.DicePalaceRabbit;
		case DicePalaceLevels.DicePalaceLight:
			return Levels.DicePalaceLight;
		case DicePalaceLevels.DicePalaceFlyingHorse:
			return Levels.DicePalaceFlyingHorse;
		case DicePalaceLevels.DicePalaceFlyingMemory:
			return Levels.DicePalaceFlyingMemory;
		case DicePalaceLevels.DicePalaceMain:
			return Levels.DicePalaceMain;
		case DicePalaceLevels.DicePalaceEightBall:
			return Levels.DicePalaceEightBall;
		}
	}
}
