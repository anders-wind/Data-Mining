package Lab3;

/***
 * The ItemSet class is used to store information concerning a single transaction.
 * Should not need any changes.
 *
 */
public class ItemSet {
	
	/***
	 * The PRIMES array is internally in the ItemSet-class' hashCode method
	 */
	private static final int[] PRIMES = { 2, 3, 5, 7, 11, 13, 17, 23, 27, 31, 37 };
    final int[] set;
    final int length;

    /***
     * Creates a new instance of the ItemSet class.
     * @param set Transaction content
     */
    public ItemSet( int[] set ) {
        this.set = set;
        this.length = set.length;
    }

    public int get(int index)
    {
        return set[index];
    }
    public void set(int index, int value)
    {
        set[index] = value;
    }
    public int getLast()
    {
        return set[length-1];
    }

    @Override
    /**
     * hashCode functioned used internally in Hashtable
     */
    public int hashCode() {
        int code = 0;
        for (int i = 0; i < set.length; i++) {
            code += set[i] * PRIMES[i];
        }
        return code;
    }

    
    @Override
    /**
     * Used to determine whether two ItemSet objects are equal
     */
    public boolean equals( Object o ) {
        if (!(o instanceof ItemSet)) {
            return false;
        }
        ItemSet other = (ItemSet) o;
        if (other.set.length != this.set.length) {
            return false;
        }
        for (int i = 0; i < set.length; i++) {
            if (set[i] != other.set[i]) {
                return false;
            }
        }
        return true;
    }
}
