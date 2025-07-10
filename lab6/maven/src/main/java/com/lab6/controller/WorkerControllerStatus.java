package com.lab6.controller;

public enum WorkerControllerStatus {
    NOT_FOUND {
        @Override
        public String toString() {
            return "not found";
        }
    },

    DONE {
        @Override
        public String toString() {
            return "done";
        }
    },

    BAD_REQUEST {
        @Override
        public String toString() {
            return "bad request";
        }
    }
}
