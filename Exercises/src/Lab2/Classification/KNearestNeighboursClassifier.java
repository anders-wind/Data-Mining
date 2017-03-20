package Lab2.Classification;

import Common.DataTypes.BooleanNominal;
import Common.Interfaces.ClassifiablePoint;
import Common.Interfaces.Classifier;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.Map;

/**
 * Created by aws on 13-02-2017.
 */
public class KNearestNeighboursClassifier implements Classifier<ClassifiablePoint<BooleanNominal>, BooleanNominal>  {

    private Collection<ClassifiablePoint<BooleanNominal>> classifiedSet;
    private int _k;

    public KNearestNeighboursClassifier(int k) {
        this.classifiedSet = new ArrayList<>();
        this._k = k;
    }

    @Override
    public void trainWithSet(Collection<ClassifiablePoint<BooleanNominal>> trainSet) {

        this.classifiedSet = trainSet;
    }

    @Override
    public BooleanNominal classify(ClassifiablePoint<BooleanNominal> elementToClassify) {
        Map<ClassifiablePoint<BooleanNominal>, Double> kNearest = new HashMap<>();
        int firstK = 0;
        for (ClassifiablePoint<BooleanNominal> classifiedElement : classifiedSet) {
            double distance = elementToClassify.keySet().stream().mapToDouble(attribute -> elementToClassify.get(attribute).distance(classifiedElement.get(attribute))).sum();
            if (firstK < _k) {
                kNearest.put(classifiedElement, distance);
            } else {
                ClassifiablePoint<BooleanNominal> worstOne = null;
                double currentDist = -1;
                for (Map.Entry<ClassifiablePoint<BooleanNominal>, Double> keyValue : kNearest.entrySet()) {
                    if (distance < keyValue.getValue() && keyValue.getValue() > currentDist) {
                        worstOne = keyValue.getKey();
                        currentDist = keyValue.getValue();
                    }
                }
                if (worstOne != null) {
                    kNearest.put(classifiedElement, distance);
                    kNearest.remove(worstOne);
                }
            }
            firstK++;
        }
        int negatives = 0, positives = 0;
        for (Map.Entry<ClassifiablePoint<BooleanNominal>, Double> keyValue : kNearest.entrySet()) {
            if (keyValue.getKey().getClassification().equals(BooleanNominal.negative()))
                negatives++;
            else
                positives++;
        }
        return negatives >= positives ? BooleanNominal.negative() : BooleanNominal.positive();
    }
}
