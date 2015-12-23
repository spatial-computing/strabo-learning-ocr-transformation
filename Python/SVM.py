__author__ = 'rmenon'

import sys
from sklearn import svm
from sklearn.multiclass import OneVsRestClassifier
from sklearn.metrics import accuracy_score
from sklearn.externals import joblib

import Data

def classify():
    file = open('result.txt', 'w')
    X_train, X_test, y_train, y_test = Data.get_dataset(0.3)
    clf = OneVsRestClassifier(svm.SVC())
    clf.fit(X_train, y_train)
    joblib.dump(clf, 'svm_model.pkl')
    y_prediction = clf.predict(X_test)
    acc = accuracy_score(y_test, y_prediction, normalize=False)
    print(acc)
    file.write(str(acc))
    file.close()
	



