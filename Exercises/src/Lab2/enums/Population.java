package Lab2.enums;

import Lab2.Interfaces.SpaceComparable;

public enum Population implements SpaceComparable<Population> {
	abundant,
	clustered,
	numerous,
	scattered,
	several,
	solitary;

	public double distance(Population comparable)
	{
		return this==comparable ? 0 : 1;
	}
}
