__author__ = 'rmenon'

import psycopg2


def connect():
    """
    Connects to PostgresSQL and returns all the training data
    :return: rows from SequenceData table
    """
    conn = psycopg2.connect("dbname='dictionary' user='postgres' host='localhost' password='Dornsife123'")
    cur = conn.cursor()
    cur.execute("""SELECT * from public.\"SequenceData\"""")
    rows = cur.fetchall()
    return rows

