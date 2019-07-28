/// <reference path="../../../../../../node_modules/@types/jasmine/index.d.ts" />
import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect } from '@angular/core/testing';
import { BrowserModule, By } from "@angular/platform-browser";
import { UploadDialogComponent } from './upload-dialog.component';

let component: UploadDialogComponent;
let fixture: ComponentFixture<UploadDialogComponent>;

describe('upload-dialog component', () => {
    beforeEach(async(() => {
        TestBed.configureTestingModule({
            declarations: [ UploadDialogComponent ],
            imports: [ BrowserModule ],
            providers: [
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ]
        });
        fixture = TestBed.createComponent(UploadDialogComponent);
        component = fixture.componentInstance;
    }));

    it('should do something', async(() => {
        expect(true).toEqual(true);
    }));
});