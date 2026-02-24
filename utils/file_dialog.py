# utils/file_dialog.py
# Cozy file dialog helpers for Paradisic Fields
# Built using a single shared braincell by Yours Truly and Grok (February 2026)

from PySide6.QtWidgets import QFileDialog
from PySide6.QtCore import Qt, QSettings
from PySide6.QtGui import QPixmap
from pathlib import Path
import logging

def open_image_dialog(parent_widget, logger=None, image_preview=None):
    if logger is None:
        logger = logging.getLogger("paradisic_fields")

    settings = QSettings("Single Shared Braincell", "Paradisic Fields")
    start_dir = settings.value("last_image_dir", str(Path.home() / "Pictures"))

    file_path, _ = QFileDialog.getOpenFileName(
        parent_widget,
        "Select a Photo",
        start_dir,
        "Images (*.jpg *.jpeg *.png *.bmp *.webp);;All Files (*)"
    )

    if file_path:
        logger.info(f"User selected image: {file_path}")
        settings.setValue("last_image_dir", str(Path(file_path).parent))

        # Show immediate preview — instant visual feedback feels magical
        if image_preview:
            from PySide6.QtGui import QPixmap
            pix = QPixmap(file_path).scaled(
                500, 500,
                Qt.KeepAspectRatio,
                Qt.SmoothTransformation
            )
            image_preview.setPixmap(pix)
            image_preview.setText("")  # clear placeholder text once image loads

        return file_path, True

    logger.debug("User cancelled file dialog")
    return None, False