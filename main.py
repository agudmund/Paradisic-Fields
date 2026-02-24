#!/usr/bin/env python3
# -*- coding: utf-8 -*-
#
# Paradisic Fields - main.py
# A minor UI for enjoying.
# Built using a single shared braincell by Yours Truly and Grok
#
# Toggle debug logging via environment variable COZY_DEBUG

import sys
import os
os.environ['TF_ENABLE_ONEDNN_OPTS'] = '0'
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '2'

from PySide6.QtWidgets import QApplication
from main_window import Paradisic
from utils.logging import setup_logging

APP_NAME = "Cozy Paradisic Fields"
APP_ORGANIZATION = "Single Shared Braincell"
APP_VERSION = "0.0.1"
DEBUG_MODE = os.getenv("COZY_DEBUG", "0") == "1"


def main() -> None:
    logger = setup_logging(debug=DEBUG_MODE)
    try:
        logger.info(f"{APP_NAME} launched (debug mode: {DEBUG_MODE})")

        app = QApplication(sys.argv)
        app.setStyle("Fusion")

        # ✨ Load our cozy central stylesheet
        style_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), "styles", "styles.qss")
        if os.path.exists(style_path):
            with open(style_path, "r", encoding="utf-8") as f:
                app.setStyleSheet(f.read())
            logger.info("🌸 Cozy stylesheet loaded successfully")
        else:
            logger.warning("⚠️  styles/styles.qss not found – continuing without custom styles")

        app.setApplicationName(APP_NAME)
        app.setOrganizationName(APP_ORGANIZATION)
        app.setApplicationVersion(APP_VERSION)

        window = Paradisic(logger=logger)
        window.show()
        sys.exit(app.exec())

    except Exception as e:  # pylint: disable=broad-exception-caught
        logger.critical(f"Starting {APP_NAME} catastrophically failed", exc_info=True)
        print(f"{APP_NAME} has entered the void: {str(e)}", file=sys.stderr)
        sys.exit(1)


if __name__ == "__main__":
    main()