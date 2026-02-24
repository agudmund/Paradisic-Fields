#!/usr/bin/env python3
# -*- coding: utf-8 -*-
#
# Paradisic Fields - main_window.py
# The cozy main UI window for enjoying
# Built using a single shared braincell by Yours Truly and Grok

from PySide6.QtWidgets import (
    QMainWindow,
    QApplication,
    QWidget,
    QVBoxLayout,
    QSplitter,
    QLabel,
    QFileDialog
)
from PySide6.QtCore import Qt, QThread, QPointF, QSettings
from PySide6.QtGui import QIcon, QPixmap
from utils.file_dialog import open_image_dialog
from utils.control_panel import create_controls_panel
import logging
import cozy as Cozy
from pathlib import Path


class Paradisic(QMainWindow):
    def __init__(self, logger=None):
        super().__init__()
        self.name = "paradisic_fields"
        self.icon = "images/appicon.png"
        self.display_name = self.name.replace("_", " ").title()

        self.logger = logger or logging.getLogger(self.name)
        self.setWindowIcon(QIcon(self.icon))
        self.setWindowTitle(self.display_name)
        self.resize(1480, 860)

        # Thread safety
        self.thread = None
        self.worker = None

        # Track currently loaded session for smart Save + auto-load on startup
        self.current_session_path = None
        self.settings = QSettings("Single Shared Braincell", "Paradisic Fields")

        # Central splitter: Left column (controls + preview) | Right canvas
        self.main_splitter = QSplitter(Qt.Horizontal)
        self.setCentralWidget(self.main_splitter)

        # === LEFT COLUMN: Controls + Image Preview ===
        left_column = QWidget()
        left_layout = QVBoxLayout(left_column)
        left_layout.setContentsMargins(20, 20, 20, 20)
        left_layout.setSpacing(20)

        controls_panel, self.load_button, self.clear_button, self.exit_button, \
        self.save_button, self.load_session_button, self.new_note_button, \
        self.delete_button, self.quick_combo, self.status_label = create_controls_panel(
            self,
            on_load_callback=self._handle_load_button,
            on_clear_callback=self._clear_preview,
            on_exit_callback=self._handle_exit,
            on_save_callback=self._handle_save_session,
            on_load_session_callback=self._handle_load_session,
            on_new_note_callback=self._handle_new_note,
            on_delete_note_callback=self._handle_delete_selected,
            on_quick_load_callback=self._handle_quick_load
        )
        left_layout.addWidget(controls_panel)

        # Image Preview
        self.image_preview = QLabel()
        self.image_preview.setObjectName("imagePreview")
        self.image_preview.setAlignment(Qt.AlignCenter)
        self.image_preview.setScaledContents(True)
        self.image_preview.setMinimumHeight(420)
        self.image_preview.setText("Single image preview\n(Load to analyze)")
        self.image_preview.setStyleSheet(
            self.image_preview.styleSheet() + "color: #8a7a67; font-style: italic;"
        )
        left_layout.addWidget(self.image_preview, stretch=1)

        # === RIGHT: Full interactive canvas from global Cozy ===
        right_panel, self.canvas_view, self.canvas_scene = Cozy.Canvas.create_canvas_panel(self)

        self.main_splitter.addWidget(left_column)
        self.main_splitter.addWidget(right_panel)
        self.main_splitter.setSizes([520, 960])
        self.main_splitter.setCollapsible(0, True)

        # Populate Quick Load combo
        self.refresh_quick_load_combo()

        # Auto-load the last session on startup (if it still exists)
        last_session = self.settings.value("last_session", "")
        if last_session and Path(last_session).exists():
            Cozy.Session.load_session(self.canvas_scene, last_session)
            self.current_session_path = last_session
            self.status_label.setText(f"Welcome back! Loaded {Path(last_session).stem} 🌱")
            self.logger.info(f"Auto-loaded last session: {last_session}")

    # ====================== LOAD ======================
    def _handle_load_button(self):
        file_path, success = open_image_dialog(
            self, logger=self.logger, image_preview=self.image_preview
        )
        if not success:
            return

        self.status_label.setText("Preparing gentle background analysis... 🌱")
        QApplication.processEvents()

        self.thread = QThread()
        self.worker = Cozy.UploadWorker(file_path)
        self.worker.moveToThread(self.thread)

        self.worker.status_updated.connect(self.status_label.setText)
        self.worker.finished.connect(self._on_analysis_finished)
        self.worker.error_occurred.connect(lambda msg: self.status_label.setText(f"Oops — {msg}"))

        self.thread.started.connect(self.worker.run)
        self.thread.finished.connect(self.thread.deleteLater)
        self.thread.start()

        self.clear_button.setEnabled(True)
        self.exit_button.setEnabled(True)

        if self.canvas_scene:
            selected_items = self.canvas_scene.selectedItems()
            if selected_items:
                for item in selected_items:
                    if isinstance(item, Cozy.WarmNode):
                        pix = QPixmap(file_path)
                        if not pix.isNull():
                            item.set_thumbnail(pix)
                            self.logger.info(f"Thumbnail added to node {item.node_id}")
                            self.status_label.setText("Thumbnail added to selected node 🌟")
                            break
            else:
                self.status_label.setText(
                    "Image loaded! Select a node on the canvas to add thumbnail 🌱"
                )

    def _on_analysis_finished(self, node_id, message):
        self.status_label.setText(message)
        self.logger.info(f"Background analysis complete for node {node_id}: {message}")

    # ====================== CLEAR ======================
    def _clear_preview(self):
        self.image_preview.clear()
        self.image_preview.setText("Single image preview\n(Load to analyze)")
        self.status_label.setText("Ready to enjoy! 🌟")
        self.clear_button.setEnabled(False)
        self.logger.debug("Preview and status cleared")

    # ====================== EXIT ======================
    def _handle_exit(self):
        self.logger.info("User chose to exit — closing Paradisic Fields with love")
        if self.thread is not None and self.thread.isRunning():
            self.thread.quit()
            if not self.thread.wait(800):
                self.thread.terminate()
                self.thread.wait()
            self.thread = None
            self.worker = None
        QApplication.instance().quit()

    # ====================== SMART SAVE ======================
    def _handle_save_session(self):
        """Smart Save: overwrites current session if loaded, otherwise asks for new name."""
        if self.current_session_path:
            Cozy.Session.save_session(self.canvas_scene, self.current_session_path)
            self.status_label.setText(f"Saved to {Path(self.current_session_path).stem} 🌟")
            self.logger.info(f"Overwrote current session: {self.current_session_path}")
        else:
            filepath, _ = QFileDialog.getSaveFileName(
                self, "Save Sketchbook Session", "", "JSON Files (*.json)"
            )
            if filepath:
                Cozy.Session.save_session(self.canvas_scene, filepath)
                self.current_session_path = filepath
                self.status_label.setText("New session saved with love! 🌟")

        # Remember this as the last session
        if self.current_session_path:
            self.settings.setValue("last_session", self.current_session_path)

        self.refresh_quick_load_combo()

    def _handle_load_session(self):
        filepath, _ = QFileDialog.getOpenFileName(
            self, "Load Sketchbook Session", "", "JSON Files (*.json)"
        )
        if filepath:
            Cozy.Session.load_session(self.canvas_scene, filepath)
            self.current_session_path = filepath
            self.settings.setValue("last_session", filepath)   # remember it
            self.status_label.setText(f"Loaded {Path(filepath).stem} 🌱")
            self.refresh_quick_load_combo()

    # ====================== QUICK LOAD ======================
    def _handle_quick_load(self, index):
        if index < 0:
            return
        filepath = self.quick_combo.itemData(index)
        if filepath:
            Cozy.Session.load_session(self.canvas_scene, filepath)
            self.current_session_path = filepath
            self.settings.setValue("last_session", filepath)
            self.status_label.setText(f"Loaded {self.quick_combo.currentText()} 🌱")

    def refresh_quick_load_combo(self):
        if not hasattr(self, 'quick_combo'):
            return
        self.quick_combo.blockSignals(True)
        self.quick_combo.clear()
        sessions = Cozy.Session.get_saved_sessions()
        for display_name, full_path in sessions:
            self.quick_combo.addItem(display_name, full_path)
        self.quick_combo.blockSignals(False)

    # ====================== NEW NOTE ======================
    def _handle_new_note(self):
        existing_ids = [item.node_id for item in self.canvas_scene.items() 
                       if isinstance(item, Cozy.WarmNode)]
        new_id = max(existing_ids, default=0) + 1

        new_pos = QPointF(80 + (new_id % 5) * 60, 80 + (new_id // 5) * 80)

        new_node = Cozy.WarmNode(
            node_id=new_id,
            full_text="New note 🌱\n\n",
            pos=new_pos
        )
        self.canvas_scene.addItem(new_node)
        self.canvas_view.centerOn(new_node)

        self.status_label.setText(f"New note {new_id} created — double-click to edit! 📝")
        self.logger.info(f"New note {new_id} added to canvas")

    # ====================== DELETE ======================
    def _handle_delete_selected(self):
        selected = self.canvas_scene.selectedItems()
        deleted = 0
        for item in selected:
            if isinstance(item, Cozy.WarmNode):
                self.canvas_scene.removeItem(item)
                deleted += 1
        if deleted > 0:
            self.status_label.setText(f"Deleted {deleted} note{'s' if deleted > 1 else ''} 🗑️")
            self.logger.info(f"Deleted {deleted} selected note(s)")
            self.canvas_scene.update()
            self.canvas_view.viewport().repaint()


if __name__ == "__main__":
    import sys
    app = QApplication(sys.argv)
    app.setStyle("Fusion")
    window = Paradisic()
    window.show()
    sys.exit(app.exec())