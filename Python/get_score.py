__author__ = 'rmenon'

import sys
import classify_gbc

def main(argv):
    classify_gbc.find_score()

if __name__=="__main__":
    main(sys.argv[1:])